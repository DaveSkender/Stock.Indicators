namespace Skender.Stock.Indicators;

// STREAM HUB (OBSERVER)

public abstract partial class StreamHub<TIn, TOut> : IStreamObserver<TIn>
{
    public bool IsSubscribed => Provider.HasSubscriber(this);

    protected IStreamObservable<TIn> Provider => provider;

    private IDisposable? Subscription { get; set; }

    // observer methods

    public virtual void OnAdd(TIn item, bool notify, int? indexHint)
    {
        // note: override when not indexed 1:1 to provide or
        // when rollback of internals is needed (late-arrivals)

        (TOut result, int index) = ToIndicator(item, indexHint);
        AppendCache(result, notify);
    }

    public virtual void OnChange(DateTime fromTimestamp)
        => Rebuild(fromTimestamp);

    public virtual void OnError(Exception exception)
        => throw exception;

    public virtual void OnCompleted()
        => Unsubscribe();

    public void Unsubscribe()
    {
        // TODO: check for thread-safety for
        // EndTransmission > OnCompleted-type race conditions
        // see https://learn.microsoft.com/en-us/dotnet/standard/events/observer-design-pattern-best-practices

        if (IsSubscribed)
        {
            Provider.Unsubscribe(this);
        }

        Subscription?.Dispose();
    }
}
