namespace Skender.Stock.Indicators;

// STREAM HUB (OBSERVABLE)

public abstract partial class StreamHub<TIn, TOut> : IStreamObservable<TOut>
{
    private readonly HashSet<IStreamObserver<TOut>> _observers = [];

    /// <summary>
    /// Stores the hub's own minimum cache size requirement (baseline).
    /// This value represents the warmup periods needed by this hub itself,
    /// independent of its subscribers.
    /// </summary>
    private int _minCacheSizeBaseline;

    // PROPERTIES

    /// <inheritdoc/>
    public virtual BinarySettings Properties { get; init; } = new(0); // default 0b00000000

    /// <inheritdoc/>
    public int MaxCacheSize { get; private set; }

    /// <inheritdoc/>
    public int MinCacheSize { get; private set; }

    /// <inheritdoc/>
    public int ObserverCount => _observers.Count;

    /// <inheritdoc/>
    public bool HasObservers => _observers.Count > 0;

    // METHODS

    /// <inheritdoc/>
    public bool HasSubscriber(IStreamObserver<TOut> observer)
        => _observers.Contains(observer);

    /// <inheritdoc/>
    public IDisposable Subscribe(IStreamObserver<TOut> observer)
    {
        _observers.Add(observer);

        // Update MinCacheSize to the maximum of all subscribers
        UpdateMinCacheSize();

        return new Unsubscriber(_observers, observer, this);
    }

    /// <inheritdoc/>
    public bool Unsubscribe(IStreamObserver<TOut> observer)
    {
        bool removed = _observers.Remove(observer);

        // Re-evaluate MinCacheSize after unsubscribing
        if (removed)
        {
            UpdateMinCacheSize();
        }

        return removed;
    }

    /// <summary>
    /// Updates the MinCacheSize to the maximum of this hub's baseline requirement
    /// and all subscribers' MinCacheSize values.
    /// </summary>
    private void UpdateMinCacheSize()
    {
        // Start from the hub's own baseline requirement
        int maxMinCacheSize = _minCacheSizeBaseline;

        foreach (IStreamObserver<TOut> observer in _observers)
        {
            if (observer is IStreamObservable<ISeries> observable)
            {
                maxMinCacheSize = Math.Max(maxMinCacheSize, observable.MinCacheSize);
            }
        }

        MinCacheSize = maxMinCacheSize;
    }

    /// <inheritdoc/>
    public void EndTransmission()
    {
        if (ObserverCount == 0)
        {
            return;
        }

        foreach (IStreamObserver<TOut> o in _observers.ToArray())
        {
            o.OnCompleted();  // subscriber removes itself
        }

        _observers.Clear();

        // Reset to baseline when all subscribers are removed
        MinCacheSize = _minCacheSizeBaseline;
    }

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
    /// <param name="hub">
    /// The parent hub that needs MinCacheSize re-evaluation on unsubscribe.
    /// </param>
    private sealed class Unsubscriber(
        ISet<IStreamObserver<TOut>> observers,
        IStreamObserver<TOut> observer,
        StreamHub<TIn, TOut> hub) : IDisposable
    {
        private readonly ISet<IStreamObserver<TOut>> _observers = observers;
        private readonly IStreamObserver<TOut> _observer = observer;
        private readonly StreamHub<TIn, TOut> _hub = hub;

        /// <summary>
        /// Remove single observer and update parent MinCacheSize.
        /// </summary>
        public void Dispose()
        {
            if (_observers.Remove(_observer))
            {
                _hub.UpdateMinCacheSize();
            }
        }
    }

    /// <summary>
    /// Sends new <c>TSeries</c> item to subscribers.
    /// </summary>
    /// <param name="item"><c>TSeries</c> item to send.</param>
    /// <param name="indexHint">Provider index hint.</param>
    private void NotifyObserversOnAdd(TOut item, int? indexHint)
    {
        if (ObserverCount == 0)
        {
            return;
        }

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
        if (ObserverCount == 0)
        {
            return;
        }

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
        if (ObserverCount == 0)
        {
            return;
        }

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
        if (ObserverCount == 0)
        {
            return;
        }

        foreach (IStreamObserver<TOut> o in _observers.ToArray())
        {
            o.OnError(exception);
        }
    }

}
