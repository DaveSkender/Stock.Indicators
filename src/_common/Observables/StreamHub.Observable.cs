namespace Skender.Stock.Indicators;

// STREAM HUB (OBSERVABLE)

public abstract partial class StreamHub<TIn, TOut> : IStreamObservable<TOut>
{
    private readonly HashSet<IStreamObserver<TOut>> _observers = [];

    public bool HasObservers => _observers.Count > 0;

    public int ObserverCount => _observers.Count;

    public IReadOnlyList<TOut> ReadCache => Cache;

    #region SUBSCRIPTION SERVICES

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
                observer.OnCompleted();
            }
        }

        _observers.Clear();
    }
    #endregion

    #region SUBSCRIBER NOTIFICATIONS

    /// <summary>
    /// Sends new <c>TSeries</c> item to subscribers
    /// </summary>
    /// <param name="item"><c>TSeries</c> item to send</param>
    /// <param name="indexHint">Provider index hint</param>
    private void NotifyObserversOnAdd(TOut item, int? indexHint)
    {
        // send to subscribers
        foreach (IStreamObserver<TOut> o in _observers.ToArray())
        {
            o.OnAdd(item, notify: true, indexHint);
        }
    }

    /// <summary>
    /// Sends rebuilds point in time to all subscribers.
    /// </summary>
    /// <param name="fromTimestamp">Rebuild starting positions</param>
    private void NotifyObserversOnChange(DateTime fromTimestamp)
    {
        foreach (IStreamObserver<TOut> o in _observers.ToArray())
        {
            o.OnChange(fromTimestamp);
        }
    }

    /// <summary>
    /// Sends error (exception) to all subscribers
    /// </summary>
    /// <param name="exception"></param>
    private void NotifyObserversOnError(Exception exception)
    {
        // send to subscribers
        foreach (IStreamObserver<TOut> o in _observers.ToArray())
        {
            o.OnError(exception);
        }
    }
    #endregion
}
