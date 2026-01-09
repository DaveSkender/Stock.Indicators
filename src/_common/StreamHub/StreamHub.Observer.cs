namespace Skender.Stock.Indicators;

// STREAM HUB (OBSERVER)

public abstract partial class StreamHub<TIn, TOut> : IStreamObserver<TIn>
{
    /// <inheritdoc/>
    public bool IsSubscribed => Provider.HasSubscriber(this);

    /// <summary>
    /// Data provider that this observer subscribes to.
    /// </summary>
    protected IStreamObservable<TIn> Provider { get; init; }

    /// <summary>
    /// Subscription token for managing the subscription lifecycle.
    /// </summary>
    private IDisposable? Subscription { get; set; }

    /// <summary>
    /// Lock object to ensure thread safety during unsubscription.
    /// </summary>
    private readonly object _unsubscribeLock = new();

    // Observer methods

    /// <inheritdoc/>
    public virtual void OnAdd(TIn item, bool notify, int? indexHint)
    {
        // Convert the input item to the output type and append it to the cache.
        // Override this method if the input and output types are not indexed 1:1.

        (TOut result, int _) = ToIndicator(item, indexHint);  // TODO: make this return array, loop appendation?
        AppendCache(result, notify);
    }

    /// <inheritdoc/>
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
}
