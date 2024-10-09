namespace Skender.Stock.Indicators;

// STREAM HUB (OBSERVER)

public abstract partial class StreamHub<TIn, TOut> : IStreamObserver<TIn>
{
    public bool IsSubscribed => Provider.HasSubscriber(this);

    protected IStreamObservable<TIn> Provider { get; init; }

    private IDisposable? Subscription { get; set; }

    // observer methods

    public virtual void OnAdd(TIn item, bool notify, int? indexHint)
    {
        // note: override when not indexed 1:1

        (TOut result, int index) = ToIndicator(item, indexHint);  // TODO: make this return array, loop appendation?
        AppendCache(result, notify);
    }

    public void OnChange(DateTime fromTimestamp)
        => Rebuild(fromTimestamp);

    public void OnError(Exception exception)
        => throw exception;

    public void OnCompleted()
        => Unsubscribe();

    public void Unsubscribe()
    {
        // TODO: check for thread-safety for EndTransmission > OnCompleted-type race conditions
        // see https://learn.microsoft.com/en-us/dotnet/standard/events/observer-design-pattern-best-practices

        if (IsSubscribed)
        {
            Provider.Unsubscribe(this);
        }

        Subscription?.Dispose();
    }
}
