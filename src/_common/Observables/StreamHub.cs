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
        = provider.GetReadOnlyCache();

    public abstract override string ToString();

    /// <summary>
    /// Delete an item from the cache.
    /// </summary>
    /// <param name="cachedItem">Cached item to delete</param>
    /// <returns cref="Act">Action taken (outcome)</returns>
    public void Remove(TOut cachedItem)
    {
        int cacheIndex = Cache.GetIndex(cachedItem, true);
        Cache.RemoveAt(cacheIndex);
        RebuildObservers(cachedItem.Timestamp);
    }

    public void RemoveAt(int cacheIndex)
    {
        TOut cacheItem = Cache[cacheIndex];
        Cache.RemoveAt(cacheIndex);
        RebuildObservers(cacheItem.Timestamp);
    }

    public void Add(TIn newIn)
        => OnNextArrival(newIn, null);

    public void Add(IEnumerable<TIn> batchIn)
    {
        foreach (TIn newIn in batchIn.ToSortedList())
        {
            OnNextArrival(newIn, null);
        }
    }

    /// <summary>
    /// Builds incremental indicator and adds to cache.
    /// </summary>
    /// <param name="item">
    /// New inbound item from provider
    /// </param>
    /// <param name="indexHint">
    /// Index position of item in provider cache or null if unknown.
    /// </param>
    protected abstract void Add(TIn item, int? indexHint);
}
