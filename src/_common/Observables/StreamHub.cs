
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

    public void OnNext((Act, TIn) value)
    {
        (Act act, TIn item) = value;

        if (act is Act.AddNew)
        {
            Add(item);
        }

        // TODO: handle revision/recursion differently
        // for different indicators; and may also need
        // to breakout OnDeleted(TIn deleted), etc.
        else
        {
            RebuildCache(item.Timestamp);
        }
    }

    public void OnError(Exception error) => throw error;

    public void OnCompleted() => Unsubscribe();

    public void Unsubscribe() => Subscription?.Dispose();

    public abstract void Add(TIn newIn);

    public void Add(IEnumerable<TIn> newIn)
    {
        foreach (TIn quote in newIn.ToSortedList())
        {
            Add(quote);
        }
    }

    public virtual Act Delete(TIn deletedIn, int? index = null)
    {
        index ??= ExactIndex(deletedIn.Timestamp);

        TOut thisItem = Cache[(int)index];

        try
        {
            Act act = Purge(thisItem);
            NotifyObservers(act, thisItem);
            return act;
        }
        catch (OverflowException)
        {
            EndTransmission();
            throw;
        }
    }
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
        int fromIndex = Cache
            .FindIndex(c => c.Timestamp >= fromTimestamp);

        // nothing to rebuild
        if (fromIndex == -1)
        {
            return;
        }

        int provIndex = Supplier.FromIndex(fromTimestamp);

        // nothing to restore
        if (provIndex == -1)
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

            provIndex = Supplier.ExactIndex(item.Timestamp);

            if (provIndex == -1)
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
            Add(Supplier.Results[i]);
        }
    }
    #endregion
}
