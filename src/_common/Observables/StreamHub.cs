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
    /// <param name="item">
    /// New inbound item from source provider
    /// </param>
    /// <param name="indexHint">Provider index hint</param>
    /// <returns>
    /// Cacheable item candidate and index hint
    /// </returns>
    protected abstract (TOut result, int index)
        ToIndicator(TIn item, int? indexHint);

    #region ADD & ANALYZE

    public void Add(TIn newIn)
        => OnAdd(newIn, null);

    public void Add(IEnumerable<TIn> batchIn)
    {
        foreach (TIn newIn in batchIn.ToSortedList())
        {
            OnAdd(newIn, null);
        }
    }

    public void Insert(TIn newIn)
    {
        // generate candidate result
        (TOut result, int index) = ToIndicator(newIn, null);

        // insert, then rebuild observers
        if (index > 0)
        {
            // note: not rebuilding self
            Cache.Insert(index, result);
            NotifyObserversOnChange(result.Timestamp);
        }

        // normal add
        else
        {
            AppendCache(result, index);
        }
    }

    /// <summary>
    /// Modify cache (attempt to add) and notify observers.
    /// </summary>
    /// <param name="result"><c>TSeries</c> item to cache</param>
    /// <param name="indexHint">Provider index hint</param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="OverflowException"></exception>
    protected void AppendCache(TOut result, int? indexHint)
    {
        try
        {
            Act act = TryAdd(result);

            // handle action taken
            switch (act)
            {
                case Act.Add:
                    NotifyObserversOnAdd(result, indexHint);
                    break;

                case Act.Ignore:
                    // duplicate found, usually
                    break;

                case Act.Rebuild:
                    Rebuild(result.Timestamp);
                    NotifyObserversOnChange(result.Timestamp);
                    break;

                // should never happen
                default:
                    throw new InvalidOperationException();
            }
        }
        catch (OverflowException ox)
        {
            NotifyObserversOnError(ox);
            throw;
        }
    }

    /// <summary>
    /// Adds item to cache.
    /// </summary>
    /// <remarks>
    /// When the cache management system cannot add the item
    /// to the cache due to an overflow condition or other issue,
    /// the caller will be given an alternate instruction.
    /// </remarks>
    /// <param name="item">Time-series object to cache</param>
    /// <returns name="act" cref="Act">Caching action</returns>
    private Act TryAdd(TOut item)
    {
        Act act = Analyze(item);

        // return alternate instruction
        if (act is not Act.Add)
        {
            return act;
        }

        // add to cache
        Cache.Add(item);
        IsFaulted = false;

        return act;
    }

    /// <summary>
    /// Analyze cache candidate to determine caching instruction.
    /// </summary>
    /// <param name="item">Cacheable time-series object</param>
    /// <returns cref="Act">Action to take</returns>
    /// <exception cref="ArgumentException">
    /// Item to modify is not found.
    /// </exception>
    /// <exception cref="OverflowException">
    /// Too many sequential duplicates were detected.
    /// </exception>
    private Act Analyze(TOut item)
    {
        // check overflow/duplicates
        if (CheckOverflow(item) is Act.Ignore)
        {
            // duplicate found
            return Act.Ignore;
        }

        // consider late-arrival (need to rebuild)
        return Cache.Count == 0 || item.Timestamp > Cache[^1].Timestamp
            ? Act.Add
            : Act.Rebuild;
    }

    /// <summary>
    /// Validate outbound item and compare to prior sent item,
    /// to gracefully manage and prevent overflow conditions.
    /// </summary>
    /// <param name="item">Cacheable time-series object</param>
    /// <returns cref="Act">
    /// A "do nothing" act instruction if duplicate or 'null'
    /// when no overflow condition is detected.
    /// </returns>
    /// <exception cref="OverflowException">
    /// Too many sequential duplicates were detected.
    /// </exception>
    private Act? CheckOverflow(TOut item)
    {
        Act? act = null;

        // skip first arrival
        if (LastItem is null)
        {
            LastItem = item;
            return act;
        }

        // check for overflow condition
        if (item.Timestamp == LastItem.Timestamp)
        {
            // note: we have a better IsEqual() comparison method below,
            // but it is too expensive as an initial quick evaluation.

            OverflowCount++;

            if (OverflowCount > 100)
            {
                const string msg = """
                   A repeated stream update exceeded the 100 attempt threshold.
                   Check and remove circular chains or check your stream provider.
                   Provider terminated.
                   """;

                IsFaulted = true;

                throw new OverflowException(msg);

                // note: overflow exception is also further handled by providers,
                // who will notify with OnError(); and then throw error to user.
            }

            // aggressive property value comparison
            if (item.Equals(LastItem))
            {
                // to prevent propagation
                // of identical cache entry
                act = Act.Ignore;
            }

            // same date with different values
            // continues as an update
            else
            {
                LastItem = item;
            }
        }
        else
        {
            OverflowCount = 0;
            LastItem = item;
        }

        return act;
    }
    #endregion

    #region REMOVE & REMOVE RANGE

    /// remove cached item
    /// <inheritdoc/>
    public void Remove(TOut cachedItem)
    {
        int cacheIndex = Cache.GetIndex(cachedItem, true);
        RemoveAt(cacheIndex);
    }

    /// remove cached item at index position
    /// <inheritdoc/>
    public void RemoveAt(int cacheIndex)
    {
        TOut cacheItem = Cache[cacheIndex];
        Cache.RemoveAt(cacheIndex);
        NotifyObserversOnChange(cacheItem.Timestamp);
    }

    /// remove cache range from timestamp
    /// <inheritdoc/>
    public void RemoveRange(DateTime fromTimestamp, bool notify)
    {
        // nothing to do
        if (Cache.Count == 0 || fromTimestamp > Cache[^1].Timestamp)
        {
            return;
        }

        // clear all
        if (fromTimestamp < Cache[0].Timestamp)
        {
            Cache.Clear();
        }

        // clear partial
        else
        {
            int fromIndex = Cache.GetIndexGte(fromTimestamp);
            Cache.RemoveRange(fromIndex, Cache.Count - fromIndex);
        }

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

        DateTime fromTimestamp;

        // clear all
        if (fromIndex <= 0)
        {
            fromTimestamp = DateTime.MinValue;
            Cache.Clear();
        }

        // clear partial
        else
        {
            fromTimestamp = Cache[fromIndex].Timestamp;
            Cache.RemoveRange(fromIndex, Cache.Count - fromIndex);
        }

        // notify observers
        if (notify)
        {
            NotifyObserversOnChange(fromTimestamp);
        }
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
    {
        Cache.Clear();
        Rebuild(-1);
    }

    // rebuild cache from timestamp
    /// <inheritdoc/>
    public void Rebuild(DateTime fromTimestamp)
    {
        int fromIndex = Cache.GetIndexGte(fromTimestamp);

        // nothing to rebuild
        if (fromIndex < 0)
        {
            return;
        }

        int provIndex = ProviderCache.GetIndexGte(fromTimestamp);
        Rebuild(fromIndex, provIndex);
    }

    // rebuild cache from index
    /// <inheritdoc/>
    public void Rebuild(int fromIndex, int? provIndex = null)
    {
        // nothing to do
        if (fromIndex >= Cache.Count)
        {
            return;
        }

        DateTime timestamp;

        // full rebuild scenario
        if (fromIndex <= 0 || Cache.Count is 0)
        {
            timestamp = DateTime.MinValue;
            provIndex = 0;
            Cache.Clear();
        }

        // partial rebuild scenario
        else
        {
            timestamp = Cache[fromIndex].Timestamp;
            provIndex ??= ProviderCache.GetIndexGte(timestamp);
            Cache.RemoveRange(fromIndex, Cache.Count - fromIndex);
        }

        // rebuild cache from provider
        if (provIndex >= 0)
        {
            for (int i = (int)provIndex; i < ProviderCache.Count; i++)
            {
                OnAdd(ProviderCache[i], i);
            }
        }

        // notify observers
        NotifyObserversOnChange(timestamp);
    }
    #endregion
}
