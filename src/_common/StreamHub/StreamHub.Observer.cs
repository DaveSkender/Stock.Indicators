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
    protected IStreamObservable<TIn> Provider { get; }

    /// <summary>
    /// Data provider's internal cache (read-only).
    /// </summary>
    protected IReadOnlyList<TIn> ProviderCache { get; }

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
        // Lock to prevent concurrent cache access.
        // ToIndicator and AppendCache access Cache, which may be modified
        // by concurrent Rebuild operations on other threads.
        // .NET locks are reentrant, so this works when called from within Rebuild.
        lock (CacheLock)
        {
            (TOut result, int _) = ToIndicator(item, indexHint);
            AppendCache(result, notify);
        }
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
        lock (CacheLock)
        {
            int removedCount = 0;
            while (Cache.Count > 0 && Cache[0].Timestamp <= toTimestamp)
            {
                Cache.RemoveAt(0);
                removedCount++;
            }

            // Allow derived classes to prune their internal state arrays
            if (removedCount > 0)
            {
                PruneState(removedCount);
            }

            // notify observers (inside lock to ensure cache consistency)
            NotifyObserversOnPrune(toTimestamp);
        }
    }

    /// <summary>
    /// Called when items are pruned from the beginning of the cache.
    /// Override this method to prune internal state arrays in sync with the cache.
    /// </summary>
    /// <param name="count">The number of items removed from the beginning of the cache.</param>
    protected virtual void PruneState(int count)
    {
        // No-op by default. Override in derived classes with internal state arrays.
    }

    /// <inheritdoc/>
    public void OnError(Exception exception)
        => throw exception;

    /// <inheritdoc/>
    public void OnCompleted()
        => Unsubscribe();

}
