namespace Skender.Stock.Indicators;

// STREAM HUB (OBSERVER)

public abstract partial class StreamHub<TIn, TOut>: IStreamObserver<TIn>
{
    public bool IsSubscribed => Provider.HasSubscriber(this);

    protected IStreamObservable<TIn> Provider => provider;

    private IDisposable? Subscription { get; set; }

    // observer methods

    public virtual void OnNextArrival(TIn item, int? indexHint)
        => Add(item, indexHint); // pass-thru to implementation

    public void OnError(Exception exception) => throw exception;

    public void OnCompleted() => Unsubscribe();

    //public void Unsubscribe() => Subscription?.Dispose();

    public void Unsubscribe()
    {
        if (IsSubscribed)
        {
            Provider.Unsubscribe(this);
        }

        Subscription?.Dispose();
    }
}
