namespace Skender.Stock.Indicators;

// STREAM OBSERVER

public class StreamObserver<TIn, TOut> : IStreamObserver<TIn>
    where TIn : struct, ISeries
    where TOut : struct, ISeries
{
    private readonly IStreamHub<TIn, TOut> _hub;
    private readonly StreamProvider<TOut> _observable;
    private readonly StreamProvider<TIn> _supplier;

    protected internal StreamObserver(
        IStreamHub<TIn, TOut> hub,
        StreamProvider<TOut> observable,
        StreamProvider<TIn> provider)
    {
        _hub = hub;
        _observable = observable;
        _supplier = provider;

        Subscription = _supplier.Subscribe(this);
    }

    public bool IsSubscribed => Subscription is not null;

    internal IDisposable? Subscription { get; set; }

    // METHODS (OBSERVER)

    public void OnNext((Act, TIn) value)
        => _hub.OnNextArrival(value.Item1, value.Item2);

    public void OnError(Exception error) => throw error;

    public void OnCompleted() => Unsubscribe();

    public void Unsubscribe() => Subscription?.Dispose();

    // METHODS (UTILITIES)

    // full reset
    /// <inheritdoc/>
    public void Reinitialize()
    {
        Unsubscribe();
        _observable.ClearCache();
        Subscription = _supplier.Subscribe(this);
    }

    // rebuild cache
    /// <inheritdoc/>
    public void RebuildCache() => RebuildCache(0);

    // rebuild cache from timestamp
    /// <inheritdoc/>
    public void RebuildCache(
        DateTime fromTimestamp)
    {
        int fromIndex = _observable.CacheP
            .FindLastIndex(c => c.Timestamp <= fromTimestamp);

        if (fromIndex == -1)
        {
            fromIndex = 0;
        }

        RebuildCache(fromIndex);
    }

    // rebuild cache from index
    /// <inheritdoc/>
    public void RebuildCache(int fromIndex)
    {
        // get equivalent provider index
        // can be different in some cases (e.g. Renko)
        int providerIndex;

        if (fromIndex <= 0)
        {
            providerIndex = 0;
        }
        else
        {
            TOut item = _observable.SpanCache[fromIndex];

            providerIndex = _supplier.CacheP
                .FindIndex(c => c.Timestamp == item.Timestamp);

            if (providerIndex == -1)
            {
                providerIndex = 0;
            }
        }

        _observable.ClearCache(fromIndex);
        _supplier.Resend(this, providerIndex, Act.AddNew);
    }
}
