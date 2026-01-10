namespace Skender.Stock.Indicators;
#pragma warning disable IDE0010 // Missing cases in switch expression

// STREAM HUB (BASE/CACHE)

/// <inheritdoc cref="IStreamHub{TIn, TOut}"/>
public abstract partial class StreamHub<TIn, TOut> : IStreamHub<TIn, TOut>
    where TIn : IReusable
    where TOut : IReusable
{
    private protected StreamHub(IStreamObservable<TIn> provider)
    {
        // store provider reference
        Provider = provider;

        // set provider cache reference
        ProviderCache = provider.GetCacheRef();

        // inherit settings (reinstantiate struct on heap)
        Properties = Properties.Combine(provider.Properties);

        // inherit max cache size
        MaxCacheSize = provider.MaxCacheSize;
    }

    /// <inheritdoc/>
    public IReadOnlyList<TOut> Results => Cache.AsReadOnly();

    /// <inheritdoc/>
    public bool IsFaulted { get; private set; }

    /// <inheritdoc/>
    public string Name { get; private protected init; } = null!;

    /// <summary>
    /// Gets the cache of stored values (base).
    /// </summary>
    internal List<TOut> Cache { get; } = [];

    /// <summary>
    /// Gets the current count of repeated caching attempts.
    /// An overflow condition is triggered after 100.
    /// </summary>
    internal byte OverflowCount { get; private set; }

    /// <summary>
    /// Gets the reference to this hub's provider's cache.
    /// </summary>
    protected IReadOnlyList<TIn> ProviderCache { get; }

    /// <summary>
    /// Gets or sets the most recent item saved to cache.
    /// </summary>
    private TOut? LastItem { get; set; }

    /// <summary>
    /// Resets the fault flag and condition.
    /// </summary>
    /// <inheritdoc/>
    public void ResetFault()
    {
        OverflowCount = 0;
        IsFaulted = false;
    }

    /// <summary>
    /// Fetches the cache reference as a read-only wrapper.
    /// </summary>
    /// <remarks>
    /// This method returns an immutable read-only wrapper that prevents modification.
    /// </remarks>
    /// <returns>The read-only cache reference.</returns>
    public IReadOnlyList<TOut> GetCacheRef() => Cache.AsReadOnly();

    /// <inheritdoc/>
    public override string ToString() => Name;

    /// <summary>
    /// Converts incremental value into an indicator candidate and cache position.
    /// </summary>
    /// <param name="item">New item from provider.</param>
    /// <param name="indexHint">Provider index hint.</param>
    /// <returns>Cacheable item candidate and index hint.</returns>
    protected abstract (TOut result, int index) ToIndicator(TIn item, int? indexHint);

    /// <summary>
    /// Adds a new item to the stream.
    /// </summary>
    /// <param name="newIn">The new item to add.</param>
    public void Add(TIn newIn) => OnAdd(newIn, notify: true, null);

    /// <summary>
    /// Adds a batch of new items to the stream.
    /// </summary>
    /// <param name="batchIn">The batch of new items to add.</param>
    public void Add(IEnumerable<TIn> batchIn)
    {
        foreach (TIn newIn in batchIn.OrderBy(static x => x.Timestamp))
        {
            OnAdd(newIn, notify: true, null);
        }
    }

    /// <summary>
    /// Inserts a new item into the stream.
    /// </summary>
    /// <param name="newIn">The new item to insert.</param>
    public void Insert(TIn newIn)
    {
        // note: should only be used when newer timestamps
        // are not impacted by the insertion of an older item

        // generate candidate result
        (TOut result, int index) = ToIndicator(newIn, null);

        // insert, then rebuild observers (no self-rebuild)
        if (index > 0)
        {
            // check overflow/duplicates
            if (IsOverflowing(result))
            {
                return; // duplicate found
            }

            Cache.Insert(index, result);
            NotifyObserversOnRebuild(result.Timestamp);
        }

        // normal add
        else
        {
            AppendCache(result, notify: true);
        }
    }

    /// <summary>
    /// Performs appropriate caching action after analysis.
    /// It will add if new, ignore if duplicate, or rebuild if late-arrival.
    /// </summary>
    /// <param name="result">Item to cache.</param>
    /// <param name="notify">Notify subscribers of change (send to observers). This is disabled for bulk operations like rebuild.</param>
    /// <exception cref="InvalidOperationException">Thrown when the operation is invalid for the current state</exception>
    protected virtual void AppendCache(TOut result, bool notify)
    {
        // check overflow/duplicates
        if (IsOverflowing(result))
        {
            return;
        }

        bool bypassRebuild = Properties[1]; // forced add/caching w/o rebuild

        // consider timeline
        Act act = bypassRebuild || Cache.Count == 0 || result.Timestamp > Cache[^1].Timestamp
            ? Act.Add
            : Act.Rebuild;

        // fulfill action
        switch (act)
        {
            // add to cache
            case Act.Add:
                Add(result, notify);
                break;

            // rebuild cache
            case Act.Rebuild:
                Rebuild(result.Timestamp);
                break;

            // would never happen
            default:
                throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Adds an item to cache and notifies observers.
    /// </summary>
    /// <param name="item">Item to add to end of cache.</param>
    /// <param name="notify">Inherited notification instructions.</param>
    private void Add(TOut item, bool notify)
    {
        // notes:
        // 1. Should only be called from AppendCache()
        // 2. Notify has to be disabled for bulk operations, like rebuild.
        // 3. Forced caching (rebuild analysis bypass) is inherited property.

        // add to cache
        Cache.Add(item);
        IsFaulted = false;

        // notify subscribers
        if (notify)
        {
            NotifyObserversOnAdd(item, Cache.Count - 1);
        }
    }

    /// <summary>
    /// Validates outbound item and compares to prior cached item,
    /// to gracefully manage and prevent overflow conditions.
    /// </summary>
    /// <param name="item">Cacheable time-series object.</param>
    /// <returns>True if item is repeating and duplicate was suppressed.</returns>
    /// <exception cref="OverflowException">Too many sequential duplicates were detected.</exception>
    private bool IsOverflowing(TOut item)
    {
        // skip first arrival
        if (LastItem is null)
        {
            LastItem = item;
            return false;
        }

        // track/check for overflow condition
        if (item.Timestamp == LastItem.Timestamp && item.Equals(LastItem))
        {
            // ^^ using progressive check to avoid Equals() on every item

            OverflowCount++;

            // handle overflow
            if (OverflowCount > 100)
            {
                const string msg = """
                    A repeated stream update exceeded the 100 attempt threshold.
                    Check and remove circular chains or check your stream provider.
                    Provider terminated.
                    """;

                IsFaulted = true;

                // emit error
                OverflowException exception = new(msg);
                NotifyObserversOnError(exception);
                throw exception;
            }

            // bypass duplicate prevention
            // when forced caching is enabled
            return !Properties[1];
        }

        // maintenance pruning
        PruneCache();

        // not repeating
        OverflowCount = 0;
        LastItem = item;
        return false;
    }

    /// <summary>
    /// Removes a cached item.
    /// </summary>
    /// <inheritdoc/>
    public void Remove(TOut cachedItem)
    {
        Cache.Remove(cachedItem);
        NotifyObserversOnRebuild(cachedItem.Timestamp);
    }

    /// <summary>
    /// Removes a cached item at a specific index position.
    /// </summary>
    /// <inheritdoc/>
    public void RemoveAt(int cacheIndex)
    {
        TOut cachedItem = Cache[cacheIndex];
        Cache.RemoveAt(cacheIndex);
        NotifyObserversOnRebuild(cachedItem.Timestamp);
    }

    /// <summary>
    /// Removes a range of cached items from a specific timestamp.
    /// </summary>
    /// <inheritdoc/>
    public virtual void RemoveRange(DateTime fromTimestamp, bool notify)
    {
        // rollback internal state
        RollbackState(fromTimestamp);

        // remove cache entries
        Cache.RemoveAll(c => c.Timestamp >= fromTimestamp);

        // notify observers
        if (notify)
        {
            NotifyObserversOnRebuild(fromTimestamp);
        }
    }

    /// <summary>
    /// Removes a range of cached items from a specific index.
    /// </summary>
    /// <inheritdoc/>
    public void RemoveRange(int fromIndex, bool notify)
    {
        // nothing to do
        if (Cache.Count == 0 || fromIndex >= Cache.Count)
        {
            return;
        }

        // remove cache entries
        DateTime fromTimestamp = fromIndex <= 0
            ? DateTime.MinValue
            : Cache[fromIndex].Timestamp;

        RemoveRange(fromTimestamp, notify);
    }

    /// <summary>
    /// Prunes the cache to the maximum size.
    /// </summary>
    protected virtual void PruneCache()
    {
        if (Cache.Count < MaxCacheSize)
        {
            return;
        }

        DateTime toTimestamp = DateTime.MinValue;

        while (Cache.Count >= MaxCacheSize)
        {
            toTimestamp = Cache[0].Timestamp;
            Cache.RemoveAt(0);
        }

        NotifyObserversOnPrune(toTimestamp);
    }

    /// <summary>
    /// Fully resets the stream hub.
    /// </summary>
    /// <inheritdoc/>
    public void Reinitialize()
    {
        Unsubscribe();
        ResetFault();
        Rebuild();
        Subscription = Provider.Subscribe(this);

        // TODO: make reinitialization abstract,
        // and build initial Cache from faster static method

        // TODO: evaluate race condition between rebuild
        // and subscribe; will it miss any high frequency data?
    }

    /// <summary>
    /// Rebuilds the cache.
    /// </summary>
    /// <inheritdoc/>
    public void Rebuild() => Rebuild(DateTime.MinValue);

    /// <summary>
    /// Rebuilds the cache from a specific timestamp.
    /// </summary>
    /// <inheritdoc/>
    public virtual void Rebuild(DateTime fromTimestamp)
    {
        // clear cache
        RemoveRange(fromTimestamp, notify: false);

        // get provider position
        int provIndex = ProviderCache.IndexGte(fromTimestamp);

        // rebuild
        if (provIndex >= 0)
        {
            for (int i = provIndex; i < ProviderCache.Count; i++)
            {
                OnAdd(ProviderCache[i], notify: false, i);
            }
        }

        // notify observers
        NotifyObserversOnRebuild(fromTimestamp);
    }

    /// <summary>
    /// Rebuilds the cache from a specific index.
    /// </summary>
    /// <inheritdoc/>
    public void Rebuild(int fromIndex)
    {
        // find timestamp
        DateTime fromTimestamp = fromIndex <= 0 || Cache.Count == 0
            ? DateTime.MinValue
            : Cache[fromIndex].Timestamp;

        // rebuild & notify
        Rebuild(fromTimestamp);
    }

    /// <summary>
    /// Rollbacks internal state to a point in time.
    /// Behavior varies by indicator.
    /// </summary>
    /// <remarks>
    /// Override when indicator needs to rollback state to a
    /// point in time (e.g. when rebuilding cache). Example:
    /// <see cref="AtrStopHub.RollbackState(DateTime)"/>
    /// </remarks>
    /// <param name="timestamp">Point in time to restore.</param>
    protected virtual void RollbackState(DateTime timestamp)
    {
        // note: override when rollback is needed
        // default: do nothing
        // see AtrStopHub() for example
    }

}
