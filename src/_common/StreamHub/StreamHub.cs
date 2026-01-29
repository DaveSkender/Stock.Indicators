namespace Skender.Stock.Indicators;
#pragma warning disable IDE0010 // Missing cases in switch expression

// STREAM HUB (BASE/CACHE)

/// <inheritdoc cref="IStreamHub{TIn, TOut}"/>
public abstract partial class StreamHub<TIn, TOut> : IStreamHub<TIn, TOut>
    where TIn : ISeries
    where TOut : ISeries
{
    /// <summary>
    /// Gets the lock object for thread-safe cache operations during rebuild/rollback.
    /// This protects against concurrent cache modifications when out-of-order
    /// data triggers rebuild operations.
    /// </summary>
    protected object CacheLock { get; } = new();

    /// <summary>
    /// Prevents self-recursion: during rebuild, OnAdd calls AppendCache,
    /// which must not trigger another rebuild on the same hub.
    /// Cascading to observers is still allowed and desired.
    /// </summary>
    private bool _isRebuilding;

    private protected StreamHub(IStreamObservable<TIn> provider)
    {
        // store provider reference
        Provider = provider;

        // provider's Cache reference (wrapped for safety)
        ProviderCache = provider.Results;

        // inherit settings (reinstantiate struct on heap)
        Properties = Properties.Combine(provider.Properties);

        // inherit max cache size from provider
        MaxCacheSize = provider.MaxCacheSize;

        // pre-allocate cache if reasonable size
        if (MaxCacheSize is > 0 and < 10_000)
        {
            Cache = new List<TOut>(MaxCacheSize);
        }

        // build read-only cache reference
        Results = Cache.AsReadOnly();
    }

    /// <summary>
    /// Validates that the inherited MaxCacheSize is sufficient for the indicator's warmup requirements.
    /// </summary>
    /// <param name="requiredWarmupPeriods">The minimum number of periods required for indicator warmup.</param>
    /// <param name="indicatorName">The name of the indicator (for error messaging).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when MaxCacheSize is less than required warmup periods.</exception>
    protected void ValidateCacheSize(int requiredWarmupPeriods, string indicatorName)
    {
        if (MaxCacheSize < requiredWarmupPeriods)
        {
            throw new ArgumentOutOfRangeException(
                nameof(requiredWarmupPeriods),
                requiredWarmupPeriods,
                $"Insufficient cache size for {indicatorName}. " +
                $"Requires at least {requiredWarmupPeriods} periods for proper initialization, " +
                $"but inherited MaxCacheSize is {MaxCacheSize}. " +
                $"Increase the provider's MaxCacheSize to at least {requiredWarmupPeriods}.");
        }

        // Set MinCacheSize to the required warmup periods
        SetMinCacheSize(requiredWarmupPeriods);
    }

    /// <summary>
    /// Sets the minimum cache size for this hub based on warmup requirements.
    /// This should be called in derived class constructors to specify the minimum
    /// number of periods required for the indicator to function correctly.
    /// </summary>
    /// <param name="requiredWarmupPeriods">The minimum number of periods required for indicator warmup.</param>
    protected void SetMinCacheSize(int requiredWarmupPeriods)
    {
        MinCacheSize = Math.Max(MinCacheSize, requiredWarmupPeriods);

        // Propagate to provider if we're increasing the requirement
        if (Provider is IStreamObservable<ISeries> observable &&
            requiredWarmupPeriods > observable.MinCacheSize)
        {
            // The provider will be updated through the Subscribe mechanism
            // when this hub subscribes, so we don't need to update it here
        }
    }

    // PROPERTIES

    /// <inheritdoc/>
    public string Name { get; private protected init; } = string.Empty;

    /// <inheritdoc/>
    public IReadOnlyList<TOut> Results { get; }

    /// <inheritdoc/>
    public bool IsFaulted { get; private set; }

    /// <summary>
    /// Gets the cache of stored values (base).
    /// </summary>
    internal List<TOut> Cache { get; } = new List<TOut>(800);

    /// <summary>
    /// Gets the current count of repeated caching attempts.
    /// An overflow condition is triggered after 100.
    /// </summary>
    internal byte OverflowCount { get; private set; }

    /// <summary>
    /// Gets or sets the most recent item saved to cache.
    /// </summary>
    private TOut? LastItem { get; set; }

    // METHODS

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
    /// Removes a cached item at a specific index position.
    /// </summary>
    /// <inheritdoc/>
    public void RemoveAt(int cacheIndex)
    {
        lock (CacheLock)
        {
            TOut cachedItem = Cache[cacheIndex];
            DateTime timestamp = cachedItem.Timestamp;
            Cache.RemoveAt(cacheIndex);

            // notify observers (inside lock to ensure cache consistency)
            NotifyObserversOnRebuild(timestamp);
        }
    }

    /// <summary>
    /// Removes a range of cached items from a specific timestamp.
    /// </summary>
    /// <inheritdoc/>
    public void RemoveRange(DateTime fromTimestamp, bool notify)
    {
        // rollback internal state
        RollbackState(fromTimestamp);

        // remove cache entries
        Cache.RemoveAll(c => c.Timestamp >= fromTimestamp);

        // reset LastItem to prevent IsOverflowing from incorrectly detecting
        // duplicates when rebuilding (the new items may have same timestamp
        // and equal values as the old LastItem, especially during warmup periods)
        LastItem = Cache.Count > 0 ? Cache[^1] : default;

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
        // Lock to prevent concurrent cache access during rebuild.
        // Out-of-order data triggers rebuild, which clears and repopulates
        // the cache. Without locking, ToIndicator may access Cache[i-1]
        // while the cache is in an inconsistent state.
        // NOTE: Observer notification must happen INSIDE the lock to prevent
        // race conditions where new items are added before rebuild notification
        // reaches observers, causing cache desynchronization.
        lock (CacheLock)
        {
            // Set flag to prevent self-recursion in AppendCache
            _isRebuilding = true;

            try
            {
                // clear cache
                RemoveRange(fromTimestamp, notify: false);

                // get provider position
                int provIndex = ProviderCache.IndexGte(fromTimestamp);

                // rebuild
                if (provIndex >= 0)
                {
                    int cacheSize = ProviderCache.Count;
                    for (int i = provIndex; i < cacheSize; i++)
                    {
                        OnAdd(ProviderCache[i], notify: false, i);
                    }
                }

                // notify observers (inside lock to ensure cache consistency
                // before any new items can be added)
                NotifyObserversOnRebuild(fromTimestamp);
            }
            finally
            {
                // Clear flag
                _isRebuilding = false;
            }
        }
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
    /// Performs appropriate caching action after analysis.
    /// It will add if new, ignore if duplicate, or rebuild if late-arrival.
    /// </summary>
    /// <param name="result">Item to cache.</param>
    /// <param name="notify">Notify subscribers of change (send to observers). This is disabled for bulk operations like rebuild.</param>
    /// <exception cref="InvalidOperationException">Thrown when the operation is invalid for the current state</exception>
    protected void AppendCache(TOut result, bool notify)
    {
        // check overflow/duplicates
        if (IsOverflowing(result))
        {
            return;
        }

        bool bypassRebuild = Properties[1]; // forced add/caching w/o rebuild

        // Prevent self-recursion: during rebuild, OnAdd processes provider items
        // and AppendCache must not trigger another rebuild on this same hub.
        // Observer cascading (NotifyObserversOnRebuild) is separate and still occurs.
        if (_isRebuilding)
        {
            bypassRebuild = true;
        }

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
        // default: do nothing
    }

    /// <summary>
    /// Prunes the cache to the maximum size.
    /// </summary>
    private void PruneCache()
    {
        if (Cache.Count < MaxCacheSize)
        {
            return;
        }

        // calculate how many items to remove
        int count = Cache.Count - MaxCacheSize + 1;

        // store timestamp of last item being removed
        DateTime toTimestamp = Cache[count - 1].Timestamp;

        // remove all items in one operation
        Cache.RemoveRange(0, count);

        NotifyObserversOnPrune(toTimestamp);
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
    /// Inserts an item without rebuilding this hub.
    /// </summary>
    /// <param name="item">Item to insert.</param>
    /// <param name="index">Cache index to insert at.</param>
    /// <param name="notify">Notify observers of rebuild.</param>
    protected void InsertWithoutRebuild(TOut item, int index, bool notify)
    {
        if (index < 0 || index > Cache.Count)
        {
            AppendCache(item, notify);
            return;
        }

        // Reject insertions that would modify indices before MinCacheSize
        // to prevent corrupted rebuilds
        if (index < MinCacheSize && Cache.Count > 0)
        {
            // Silently ignore the insert to maintain stability
            return;
        }

        // Reject insertions that precede the current cache timeline
        if (Cache.Count > 0 && item.Timestamp < Cache[0].Timestamp)
        {
            // Silently ignore inserts before the cache timeline
            return;
        }

        if (IsOverflowing(item))
        {
            return;
        }

        if (index < Cache.Count && Cache[index].Timestamp == item.Timestamp)
        {
            if (Cache[index].Equals(item))
            {
                return;
            }

            Cache[index] = item;
        }
        else
        {
            Cache.Insert(index, item);
        }

        if (notify)
        {
            NotifyObserversOnRebuild(item.Timestamp);
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



}
