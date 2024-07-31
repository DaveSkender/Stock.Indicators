namespace Skender.Stock.Indicators;

// STREAM HUB (ADD/REMOVE)

public abstract partial class StreamHub<TIn, TOut>
{
    public void Add(TIn newIn)
        => OnNext((Act.Unknown, newIn, null));

    public void Add(IEnumerable<TIn> batchIn)
    {
        foreach (TIn newIn in batchIn.ToSortedList())
        {
            OnNext((Act.Unknown, newIn, null));
        }
    }

    public Act Remove(TOut cachedItem)
    {
        Act? act;

        // handle overflow condition
        try
        {
            act = CheckOverflow(cachedItem);
        }
        catch (OverflowException)
        {
            EndTransmission();
            throw;
        }

        // handle duplicates
        if (act is Act.Ignore)
        {
            return Act.Ignore;
        }

        // remove/rebuild
        return RemoveAt(GetIndex(cachedItem, true));
    }

    public Act RemoveAt(int cacheIndex)
    {
        RebuildCache(cacheIndex);
        return Act.Rebuild;
    }

    /// <summary>
    /// Builds incremental indicator and adds to cache.
    /// </summary>
    /// <remarks>
    /// It is expected that pre-cache analysis has already been done
    /// and "new" is the most expected action to take.
    /// </remarks>
    /// <param name="item">
    /// New inbound item from provider
    /// </param>
    /// <param name="index">
    /// Index position of item in provider cache or null if unknown).
    /// </param>
    internal abstract void Add(TIn item, int? index);
}
