namespace Skender.Stock.Indicators;

// STREAM HUB (OBSERVER)

public abstract partial class StreamHub<TIn, TOut> : IStreamObserver<TIn>
{
    // PROPERTIES

    /// <inheritdoc/>
    public bool IsSubscribed => Provider.HasSubscriber(this);

    /// <summary>
    /// Data provider that this observer subscribes to.
    /// </summary>
    protected IStreamObservable<TIn> Provider { get; private protected init; }

    /// <summary>
    /// Data provider's internal cache (read-only).
    /// </summary>
    /// <remarks>This is an alias for <see cref="Provider"/>.ReadCache</remarks>
    protected IReadOnlyList<TIn> ProviderCache => Provider.ReadCache;

    /// <summary>
    /// Subscription token for managing the subscription lifecycle.
    /// </summary>
    private IDisposable? Subscription { get; set; }

    /// <summary>
    /// Lock object to ensure thread safety during unsubscription.
    /// </summary>
    private readonly object _unsubscribeLock = new();

    // METHODS

    /// <inheritdoc/>
    public void Unsubscribe()
    {
        // Ensure thread-safety for EndTransmission > OnCompleted-type race conditions
        // see https://learn.microsoft.com/en-us/dotnet/standard/events/observer-design-pattern-best-practices

        lock (_unsubscribeLock)
        {
            if (IsSubscribed)
            {
                Provider.Unsubscribe(this);
            }

            Subscription?.Dispose();
            Subscription = null; // ensure the ref is cleared
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Override this method if the input and output types are not indexed 1:1.
    /// </remarks>
    public virtual void OnAdd(TIn item, bool notify, int? indexHint)
    {
        (TOut result, int _) = ToIndicator(item, indexHint);
        AppendCache(result, notify);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Override this method if complex state rollback or rebuild logic is required.
    /// </remarks>
    public virtual void OnRebuild(DateTime fromTimestamp)
        => Rebuild(fromTimestamp);

    /// <inheritdoc/>
    public void OnPrune(DateTime toTimestamp)
    {
        while (Cache.Count > 0 && Cache[0].Timestamp <= toTimestamp)
        {
            Cache.RemoveAt(0);
        }

        // notify observers
        NotifyObserversOnPrune(toTimestamp);
    }

    /// <inheritdoc/>
    public void OnError(Exception exception)
        => throw exception;

    /// <inheritdoc/>
    public void OnCompleted()
        => Unsubscribe();

}
