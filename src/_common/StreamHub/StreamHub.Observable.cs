namespace Skender.Stock.Indicators;

// STREAM HUB (OBSERVABLE)

public abstract partial class StreamHub<TIn, TOut> : IStreamObservable<TOut>
{
    private readonly HashSet<IStreamObserver<TOut>> _observers = [];

    /// <inheritdoc/>
    public bool HasObservers => _observers.Count > 0;

    /// <inheritdoc/>
    public int ObserverCount => _observers.Count;

    /// <inheritdoc/>
    public IReadOnlyList<TOut> ReadCache => Cache.AsReadOnly();

    /// <inheritdoc/>
    public int MaxCacheSize { get; init; }

    /// <inheritdoc/>
    public virtual BinarySettings Properties { get; init; } = new(0); // default 0b00000000

    /// <inheritdoc/>
    public IDisposable Subscribe(IStreamObserver<TOut> observer)
    {
        _observers.Add(observer);
        return new Unsubscriber(_observers, observer);
    }

    /// <inheritdoc/>
    public bool Unsubscribe(IStreamObserver<TOut> observer)
        => _observers.Remove(observer);

    /// <inheritdoc/>
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
    private sealed class Unsubscriber(
        ISet<IStreamObserver<TOut>> observers,
        IStreamObserver<TOut> observer) : IDisposable
    {
        private readonly ISet<IStreamObserver<TOut>> _observers = observers;
        private readonly IStreamObserver<TOut> _observer = observer;

        /// <summary>
        /// Remove single observer.
        /// </summary>
        public void Dispose() => _observers.Remove(_observer);
    }

    /// <inheritdoc/>
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

    /// <summary>
    /// Sends new <c>TSeries</c> item to subscribers.
    /// </summary>
    /// <param name="item"><c>TSeries</c> item to send.</param>
    /// <param name="indexHint">Provider index hint.</param>
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
    /// <param name="fromTimestamp">Rebuild starting date.</param>
    protected void NotifyObserversOnRebuild(DateTime fromTimestamp)
    {
        foreach (IStreamObserver<TOut> o in _observers.ToArray())
        {
            o.OnRebuild(fromTimestamp);
        }
    }

    /// <summary>
    /// Sends prune notification to all subscribers.
    /// </summary>
    /// <param name="toTimestamp">Prune ending date.</param>
    private void NotifyObserversOnPrune(DateTime toTimestamp)
    {
        foreach (IStreamObserver<TOut> o in _observers.ToArray())
        {
            o.OnPrune(toTimestamp);
        }
    }

    /// <summary>
    /// Sends error (exception) to all subscribers.
    /// </summary>
    /// <param name="exception">The exception to send.</param>
    private void NotifyObserversOnError(Exception exception)
    {
        // send to subscribers
        foreach (IStreamObserver<TOut> o in _observers.ToArray())
        {
            o.OnError(exception);
        }
    }

}
