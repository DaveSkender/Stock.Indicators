
namespace Skender.Stock.Indicators;

// STREAM HUB (OBSERVER BASE) - observer members only
// with SUBJECT BASE: PROVIDER and CACHE

public abstract class StreamHub<TIn, TOut>(
    IStreamProvider<TIn> provider
) : StreamProvider<TOut>, IStreamHub<TIn, TOut>
    where TIn : ISeries
    where TOut : ISeries
{
    public bool IsSubscribed => Subscription is not null;

    internal IDisposable? Subscription { get; set; }

    protected IStreamProvider<TIn> Supplier => provider;

    #region METHODS (OBSERVER)

    public abstract override string ToString();

    public void OnNext((Act, TIn, int?) value)
    {
        (Act act, TIn item, int? index) = value;

        if (act is Act.Unknown or Act.AddNew)
        {
            Add(act, item, index);
            return;
        }

        // TODO: handle revision/recursion differently
        // for different indicators; and may also need
        // to breakout OnDeleted(TIn deleted), etc.
        RebuildCache(item.Timestamp);
    }

    public void OnError(Exception error) => throw error;

    public void OnCompleted() => Unsubscribe();

    public void Unsubscribe() => Subscription?.Dispose();

    public void Add(TIn newIn) => OnNext((Act.Unknown, newIn, null));

    public void Add(IEnumerable<TIn> newIn)
    {
        foreach (TIn quote in newIn.ToSortedList())
        {
            OnNext((Act.Unknown, quote, null));
        }
    }

    public virtual Act Delete(TOut cachedItem)
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

    #endregion

    #region METHODS (CACHE REBUILD)

    // full reset
    /// <inheritdoc/>
    public void Reinitialize()
    {
        Unsubscribe();
        ResetFault();
        ClearCache();
        Subscription = Supplier.Subscribe(this);
        RebuildCache();
    }

    // rebuild cache
    /// <inheritdoc/>
    public void RebuildCache() => RebuildCache(0);

    // rebuild cache from timestamp
    /// <inheritdoc/>
    public void RebuildCache(
        DateTime fromTimestamp)
    {
        int fromIndex = GetInsertIndex(fromTimestamp);

        // nothing to rebuild
        if (fromIndex < 0)
        {
            return;
        }

        int provIndex = Supplier.GetInsertIndex(fromTimestamp);

        // nothing to restore
        if (provIndex < 0)
        {
            provIndex = int.MaxValue;
        }

        RebuildCache(fromIndex, provIndex);
    }

    // rebuild cache from index
    /// <inheritdoc/>
    public void RebuildCache(int fromIndex)
    {
        // get equivalent provider index
        int provIndex;

        if (fromIndex <= 0)
        {
            provIndex = 0;
        }
        else
        {
            TOut item = Cache[fromIndex];

            provIndex = Supplier.GetInsertIndex(item.Timestamp);

            if (provIndex < 0)
            {
                // nothing to restore
                provIndex = int.MaxValue;
            }
        }

        RebuildCache(fromIndex, provIndex);
    }

    /// <summary>
    /// Rebuild cache from index and provider index positions.
    /// </summary>
    /// <param name="thisIndex">Cache starting position to purge.</param>
    /// <param name="provIndex">Provider starting position to add back.</param>
    private void RebuildCache(int thisIndex, int provIndex)
    {
        // clear outdated cache
        ClearCache(thisIndex);

        // rebuild cache from provider
        for (int i = provIndex; i < Supplier.Results.Count; i++)
        {
            OnNext((Act.AddNew, Supplier.Results[i], i));
        }
    }

    #endregion
}
