namespace Skender.Stock.Indicators;

// STREAM HUB (BASE)

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
    protected IReadOnlyList<TIn> ProviderCache { get; }
        = provider.GetCacheRef();

    public abstract override string ToString();

    public void Add(TIn newIn)
        => OnNextAddition(newIn, null);

    public void Add(IEnumerable<TIn> batchIn)
    {
        foreach (TIn newIn in batchIn.ToSortedList())
        {
            OnNextAddition(newIn, null);
        }
    }

    public void Insert(TIn newIn)
    {
        // generate candidate result
        (TOut result, int? index) = ToCandidate(newIn, null);

        // insert, then rebuild observers
        if (index > 0)
        {
            // note: not rebuilding self
            Cache.Insert((int)index, result);
            RebuildObservers(result.Timestamp);
        }

        // normal add
        else
        {
            Motify(result, index);
        }
    }

    public void Remove(TOut cachedItem)
    {
        int cacheIndex = Cache.GetIndex(cachedItem, true);
        RemoveAt(cacheIndex);
    }

    public void RemoveAt(int cacheIndex)
    {
        TOut cacheItem = Cache[cacheIndex];
        Cache.RemoveAt(cacheIndex);
        RebuildObservers(cacheItem.Timestamp);
    }

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
    protected abstract (TOut result, int? index)
        ToCandidate(TIn item, int? indexHint);
}
