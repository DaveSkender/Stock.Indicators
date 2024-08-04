namespace Skender.Stock.Indicators;

// STREAM HUB (OBSERVABLE)

public abstract partial class StreamHub<TIn, TOut> : IStreamObservable<TOut>
{
    private readonly HashSet<IStreamObserver<TOut>> _observers = [];

    public bool HasObservers => _observers.Count > 0;

    public int ObserverCount => _observers.Count;

    public IReadOnlyList<TOut> ReadCache => Cache;

    // SUBSCRIPTION SERVICES

    // subscribe observer
    public IDisposable Subscribe(IStreamObserver<TOut> observer)
    {
        _observers.Add(observer);
        return new Unsubscriber(_observers, observer);
    }

    public bool Unsubscribe(IStreamObserver<TOut> observer)
        => _observers.Remove(observer);

    // check if observer is subscribed
    public bool HasSubscriber(IStreamObserver<TOut> observer)
        => _observers.Contains(observer);

    /// <summary>
    /// A disposable subscription to the stream provider.
    /// <para>Unsubscribed with <see cref="Dispose()"/></para>
    /// </summary>
    /// <param name="observers">
    /// Registry of all subscribers (by ref)
    /// </param>
    /// <param name="observer">
    /// Your unique subscription as provided.
    /// </param>
    private class Unsubscriber(
        ISet<IStreamObserver<TOut>> observers,
        IStreamObserver<TOut> observer) : IDisposable
    {
        // remove single observer
        public void Dispose() => observers.Remove(observer);
    }

    // unsubscribe all observers
    public void EndTransmission()
    {
        foreach (IStreamObserver<TOut> observer
            in _observers.ToArray())
        {
            if (_observers.Contains(observer))
            {
                // subscriber removes itself
                observer.OnStopped();
            }
        }

        _observers.Clear();
    }
}
