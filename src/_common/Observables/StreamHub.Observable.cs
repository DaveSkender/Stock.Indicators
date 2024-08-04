namespace Skender.Stock.Indicators;

// STREAM HUB (OBSERVABLE)

public abstract partial class StreamHub<TIn, TOut> : IStreamObservable<TOut>
{
    private readonly HashSet<IStreamObserver<TOut>> _observers = [];

    public bool HasSubscribers => _observers.Count > 0;

    public int SubscriberCount => _observers.Count;

    public IReadOnlyList<TOut> ReadCache => Cache;

    // SUBSCRIPTION SERVICES

    // subscribe observer
    public IDisposable Subscribe(IStreamObserver<TOut> observer)
    {
        _observers.Add(observer);
        return new Unsubscriber(_observers, observer);
    }

    public void Unsubscribe(IStreamObserver<TOut> observer)
    {
        _observers.Remove(observer);
    }

    // check if observer is subscribed
    public bool HasSubscriber(IStreamObserver<TOut> observer)
        => _observers.Contains(observer);

    /// <summary>
    /// A disposable subscription to the stream provider.
    /// <para>Unsubscribed with <see cref="Dispose()"/></para>
    /// </summary>
    /// <param name="subscribers">
    /// Registry of all subscribers (by ref)
    /// </param>
    /// <param name="subscriber">
    /// Your unique subscription as provided.
    /// </param>
    private class Unsubscriber(
        ISet<IStreamObserver<TOut>> subscribers,
        IStreamObserver<TOut> subscriber) : IDisposable
    {
        // remove single observer
        public void Dispose() => subscribers.Remove(subscriber);
    }

    // unsubscribe all observers
    public void EndTransmission()
    {
        foreach (IStreamObserver<TOut> subscriber
            in _observers.ToArray())
        {
            if (_observers.Contains(subscriber))
            {
                subscriber.OnCompleted();
            }
        }

        _observers.Clear();
    }
}
