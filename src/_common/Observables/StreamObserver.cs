namespace Skender.Stock.Indicators;

// STREAM OBSERVER

public class StreamObserver<TIn, TOut> : IStreamObserver<TIn>
    where TIn : struct, ISeries
    where TOut : struct, ISeries
{
    private readonly IObserverHub<TIn, TOut> _hub;
    private readonly StreamCache<TOut> _cache;
    private readonly StreamProvider<TOut> _observable;
    private readonly StreamProvider<TIn> _supplier;

    protected internal StreamObserver(
        IObserverHub<TIn, TOut> hub,
        StreamCache<TOut> cache,
        StreamProvider<TOut> observable,
        StreamProvider<TIn> provider)
    {
        _hub = hub;
        _cache = cache;
        _observable = observable;
        _supplier = provider;
    }

    public bool IsSubscribed => Subscription is not null;

    internal IDisposable? Subscription { get; set; }

    // METHODS (OBSERVER)

    public void OnNext((Act, TIn) value)
    {
        (Act act, TIn item) = value;

        if (act is Act.AddNew)
        {
            _hub.OnNextNew(item);
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

    // METHODS (UTILITIES)

    // full reset
    /// <inheritdoc/>
    public void Reinitialize()
    {
        Unsubscribe();
        _cache.Reset();
        _observable.ClearCache();
        Subscription = _supplier.Subscribe(this);
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
        int fromIndex = _observable.StreamCache.Cache
            .FindIndex(c => c.Timestamp >= fromTimestamp);

        // nothing to rebuild
        if (fromIndex == -1)
        {
            return;
        }

        int provIndex = _supplier.StreamCache.Cache
            .FindIndex(c => c.Timestamp >= fromTimestamp);

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
            TOut item = _observable.StreamCache.ReadCache[fromIndex];

            provIndex = _supplier.StreamCache.Cache
                .FindIndex(c => c.Timestamp >= item.Timestamp);

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
        _observable.ClearCache(thisIndex);

        // rebuild cache from provider
        for (int i = provIndex; i < _supplier.StreamCache.Cache.Count; i++)
        {
            _hub.OnNextNew(_supplier.StreamCache.ReadCache[i]);
        }
    }
}
