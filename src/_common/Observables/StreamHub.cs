namespace Skender.Stock.Indicators;

// STREAM HUB (BASE/CACHE)

#region chain and quote variants

public abstract class QuoteProvider<TIn, TOut>(
    IStreamObservable<TIn> provider
) : StreamHub<TIn, TOut>(provider), IQuoteProvider<TOut>
    where TIn : IReusable
    where TOut : IQuote
{
    public IReadOnlyList<TOut> Quotes => Cache;
};

public abstract class ChainProvider<TIn, TOut>(
    IStreamObservable<TIn> provider
) : StreamHub<TIn, TOut>(provider), IChainProvider<TOut>
    where TIn : IReusable
    where TOut : IReusable;
#endregion

/// <summary>
/// Streaming hub (abstract observer/provider)
/// </summary>
public abstract partial class StreamHub<TIn, TOut>(
    IStreamObservable<TIn> provider
) : IStreamHub<TIn, TOut>
    where TIn : ISeries
    where TOut : ISeries
{
    #region PROPERTIES

    /// <inheritdoc/>
    public IReadOnlyList<TOut> Results => Cache;

    /// <inheritdoc/>
    public bool IsFaulted { get; private set; }

    /// <summary>
    /// Cache of stored values (base).
    /// </summary>
    internal List<TOut> Cache { get; } = [];

    /// <summary>
    /// Current count of repeated caching attempts.
    /// An overflow condition is triggered after 100.
    /// </summary>
    internal byte OverflowCount { get; private set; }

    /// <summary>
    /// Reference to this hub's provider's cache.
    /// </summary>
    protected IReadOnlyList<TIn> ProviderCache { get; }
        = provider.GetCacheRef();

    /// <summary>
    /// Most recent item saved to cache.
    /// </summary>
    private TOut? LastItem { get; set; }

    #endregion

    // reset fault flag and condition
    /// <inheritdoc/>
    public void ResetFault()
    {
        OverflowCount = 0;
        IsFaulted = false;
    }

    // fetch cache reference
    /// <inheritdoc/>
    public IReadOnlyList<TOut> GetCacheRef() => Cache;

    public abstract override string ToString();

    /// <summary>
    /// Converts incremental value into
    /// an indicator candidate and cache position.
    /// </summary>
    /// <param name="item">New item from provider</param>
    /// <param name="indexHint">Provider index hint</param>
    /// <returns>Cacheable item candidate and index hint</returns>
    protected abstract (TOut result, int index)
        ToIndicator(TIn item, int? indexHint);

    #region ADD & ANALYZE

    public void Add(TIn newIn)
        => OnAdd(newIn, notify: true, null);

    public void Add(IEnumerable<TIn> batchIn)
    {
        foreach (TIn newIn in batchIn.ToSortedList())
        {
            OnAdd(newIn, notify: true, null);
        }
    }

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
            NotifyObserversOnChange(result.Timestamp);
        }

        // normal add
        else
        {
            AppendCache(result, notify: true);
        }
    }

    /// <summary>
    /// Add item to cache and notify observers.
    /// </summary>
    /// <param name="item">Item to add to end of cache</param>
    /// <param name="notify">Notify subscribers of new item</param>
    private void Add(TOut item, bool notify)
    {
        /* notes:
         * 
         * 1. Should only be called from AppendCache()
         * 2. Notify is optional to support rebuild case
         *    that needs to add many without incremental notifying,
         *    to avoid excessive cascading impacts. */

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
    /// Perform appropriate caching action after analysis.
    /// It will add if new, ignore if duplicate, or rebuild if late-arrival.
    /// </summary>
    /// <param name="result"><c>TSeries</c> item to cache.</param>
    /// <param name="notify">Notify subscribers of change, if appropriate.</param>
    /// <exception cref="InvalidOperationException"></exception>
    protected void AppendCache(TOut result, bool notify)
    {
        // check overflow/duplicates
        if (IsOverflowing(result))
        {
            return;
        }

        // consider timeline
        Act act = Cache.Count == 0 || result.Timestamp > Cache[^1].Timestamp
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

            // should never happen
            default:
                throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Validate outbound item and compare to prior cached item,
    /// to gracefully manage and prevent overflow conditions.
    /// </summary>
    /// <param name="item">Cacheable time-series object</param>
    /// <returns>True if item is repeating.</returns>
    /// <exception cref="OverflowException">
    /// Too many sequential duplicates were detected.
    /// </exception>
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

            return true;
        }

        // not repeating
        OverflowCount = 0;
        LastItem = item;
        return false;
    }
    #endregion

    #region REMOVE & REMOVE RANGE

    /// remove cached item
    /// <inheritdoc/>
    public void Remove(TOut cachedItem)
    {
        Cache.Remove(cachedItem);
        NotifyObserversOnChange(cachedItem.Timestamp);
    }

    /// remove cached item at index position
    /// <inheritdoc/>
    public void RemoveAt(int cacheIndex)
    {
        TOut cachedItem = Cache[cacheIndex];
        Cache.RemoveAt(cacheIndex);
        NotifyObserversOnChange(cachedItem.Timestamp);
    }

    /// remove cache range from timestamp
    /// <inheritdoc/>
    public void RemoveRange(DateTime fromTimestamp, bool notify)
    {
        Cache.RemoveAll(c => c.Timestamp >= fromTimestamp);

        // notify observers
        if (notify)
        {
            NotifyObserversOnChange(fromTimestamp);
        }
    }

    /// remove cache range from index
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
    #endregion

    #region REBUILD & REINITIALIZE

    // full reset
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

    // rebuild cache
    /// <inheritdoc/>
    public void Rebuild()
        => Rebuild(DateTime.MinValue);

    // rebuild cache from timestamp
    /// <inheritdoc/>
    public void Rebuild(DateTime fromTimestamp)
    {
        // clear cache
        RemoveRange(fromTimestamp, notify: false);

        // get provider position
        int provIndex = ProviderCache.GetIndexGte(fromTimestamp);

        // rebuild cache
        if (provIndex >= 0)
        {
            for (int i = provIndex; i < ProviderCache.Count; i++)
            {
                OnAdd(ProviderCache[i], notify: false, i);
            }
        }

        // notify observers
        NotifyObserversOnChange(fromTimestamp);
    }

    // rebuild cache from index
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
    #endregion
}
