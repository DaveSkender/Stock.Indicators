
namespace Skender.Stock.Indicators;

// STREAM HUB (ADD/REMOVE)

public abstract partial class StreamHub<TIn, TOut>
{
    public void Add(TIn newIn)
        => OnNext((Act.Unknown, newIn, null));

    public void Add(IEnumerable<TIn> newIn)
    {
        foreach (TIn quote in newIn.ToSortedList())
        {
            OnNext((Act.Unknown, quote, null));
        }
    }

    public virtual Act Remove(TOut cachedItem)
        => RemoveAt(GetIndex(cachedItem, false));

    public Act RemoveAt(int cacheIndex)
    {
        TOut thisItem = Cache[cacheIndex];

        try
        {
            Act act = Purge(cacheIndex);
            NotifyObservers(act, thisItem, cacheIndex);
            return act;
        }
        catch (OverflowException)
        {
            EndTransmission();
            throw;
        }
    }

    /// <summary>
    /// Builds incremental indicator and adds to cache.
    /// </summary>
    /// <param name="act">
    /// Caching instruction hint from provider
    /// </param>
    /// <param name="item">
    /// New inbound item from provider
    /// </param>
    /// <param name="index">
    /// Index position of item in provider cache
    /// </param>
    internal abstract void Add(Act act, TIn item, int? index);
}
