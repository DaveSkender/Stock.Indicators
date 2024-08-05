namespace Skender.Stock.Indicators;

// STREAM HUB (OBSERVER)

public abstract partial class StreamHub<TIn, TOut> : IStreamObserver<TIn>
{
    public bool IsSubscribed => Provider.HasSubscriber(this);

    protected IStreamObservable<TIn> Provider => provider;

    private IDisposable? Subscription { get; set; }

    // observer methods

    public virtual void OnNextAddition(TIn item, int? indexHint)
    {
        // pass-thru, usually
        (TOut result, int index) = ToCandidate(item, indexHint);
        AppendCache(result, index);
    }

    public void OnNextRemoval(DateTime timestamp)
        => RebuildCache(timestamp);

    public void OnError(Exception exception)
    {
        OnStopped();
        throw exception;
    }

    public void OnStopped()
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
