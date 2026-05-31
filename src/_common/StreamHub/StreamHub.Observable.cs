namespace Skender.Stock.Indicators;

// STREAM HUB (OBSERVABLE)

public abstract partial class StreamHub<TIn, TOut> : IStreamObservable<TOut>
{
    private readonly HashSet<IStreamObserver<TOut>> _observers = [];

    /// <summary>
    /// Baseline minimum cache size requirement for this hub (set by ValidateCacheSize).
    /// </summary>
    private int _minCacheSizeBaseline = 0;

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

    // Observer isolation: a faulting subscriber must not abort notification to
    // its siblings. Each callback below is guarded; a thrown exception is routed
    // to that subscriber's own OnError and the fan-out continues. A failure
    // inside OnError itself is intentionally swallowed (it cannot be routed
    // anywhere without recursing). Catching the general Exception type and the
    // deliberate swallow are required for an isolation boundary, hence the
    // CA1031 + RCS1075 suppressions scoped to this region.
#pragma warning disable CA1031, RCS1075 // intentional catch-all at an observer-isolation boundary

    /// <summary>
    /// Sends new <c>TSeries</c> item to subscribers.
    /// </summary>
    /// <param name="item"><c>TSeries</c> item to send.</param>
    /// <param name="indexHint">Provider index hint.</param>
    protected void NotifyObserversOnAdd(TOut item, int? indexHint)
    {
        if (ObserverCount == 0)
        {
            return;
        }

        foreach (IStreamObserver<TOut> o in _observers.ToArray())
        {
            try
            {
                o.OnAdd(item, notify: true, indexHint);
            }
            catch (Exception ex)
            {
                IsolateObserverFault(o, ex);
            }
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
            try
            {
                o.OnRebuild(fromTimestamp);
            }
            catch (Exception ex)
            {
                IsolateObserverFault(o, ex);
            }
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
            try
            {
                o.OnPrune(toTimestamp);
            }
            catch (Exception ex)
            {
                IsolateObserverFault(o, ex);
            }
        }
    }

    /// <summary>
    /// Sends error (exception) to all subscribers.
    /// </summary>
    /// <param name="exception">Exception to send.</param>
    private void NotifyObserversOnError(Exception exception)
    {
        if (ObserverCount == 0)
        {
            return;
        }

        foreach (IStreamObserver<TOut> o in _observers.ToArray())
        {
            try
            {
                o.OnError(exception);
            }
            catch (Exception)
            {
                // The subscriber's own error handler threw; suppress so the
                // remaining subscribers are still notified of the fault.
            }
        }
    }

    /// <summary>
    /// Routes a faulting observer callback to that observer's own
    /// <see cref="IStreamObserver{T}.OnError"/>, isolated so a failure inside the
    /// handler cannot abort notification to sibling observers.
    /// </summary>
    /// <param name="observer">The observer whose callback threw.</param>
    /// <param name="exception">The exception thrown by the callback.</param>
    private static void IsolateObserverFault(IStreamObserver<TOut> observer, Exception exception)
    {
        try
        {
            observer.OnError(exception);
        }
        catch (Exception)
        {
            // The faulting observer's error handler also threw; suppress to keep
            // sibling observers isolated from it.
        }
    }

#pragma warning restore CA1031, RCS1075 // intentional catch-all at an observer-isolation boundary

}
