namespace Skender.Stock.Indicators;

#pragma warning disable CA1062 // Validate arguments of public methods

// STREAM HUB (STATE MANAGEMENT)
/// <summary>
/// StreamHub with state management for stateful indicators.
/// </summary>
/// <typeparam name="TIn">Type of inbound provider data.</typeparam>
/// <typeparam name="TState">Type of state object for inter-candle computation state.</typeparam>
/// <typeparam name="TOut">Type of outbound indicator data.</typeparam>
public abstract class StreamHubState<TIn, TState, TOut> : StreamHub<TIn, TOut>
    where TIn : ISeries
    where TOut : ISeries
    where TState : IHubState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StreamHubState{TIn, TState, TOut}"/> class.
    /// </summary>
    /// <param name="provider">The stream provider.</param>
    protected StreamHubState(IStreamObservable<TIn> provider)
        : base(provider)
    {
    }

    /// <summary>
    /// Gets the cache of stored state values.
    /// </summary>
    internal List<TState> StateCache { get; } = [];

    /// <summary>
    /// Temporarily stores the state from the last ToIndicatorState call.
    /// </summary>
    private TState? _pendingState;

    /// <summary>
    /// Converts incremental value into an indicator candidate, state, and cache position.
    /// </summary>
    /// <param name="item">New item from provider.</param>
    /// <param name="indexHint">Provider index hint.</param>
    /// <returns>Cacheable item candidate, state, and index hint.</returns>
    protected abstract (TOut result, TState state, int index) ToIndicatorState(TIn item, int? indexHint);

    /// <summary>
    /// Converts incremental value into an indicator candidate and cache position.
    /// This implementation calls ToIndicatorState and temporarily stores the state.
    /// </summary>
    /// <param name="item">New item from provider.</param>
    /// <param name="indexHint">Provider index hint.</param>
    /// <returns>Cacheable item candidate and index hint.</returns>
    protected sealed override (TOut result, int index) ToIndicator(TIn item, int? indexHint)
    {
        // Call the state-aware method
        (TOut result, TState state, int index) = ToIndicatorState(item, indexHint);

        // Store state temporarily - it will be committed in AppendCache
        _pendingState = state;

        return (result, index);
    }

    /// <summary>
    /// Adds an item to cache and stores corresponding state.
    /// </summary>
    /// <param name="result">Item to cache.</param>
    /// <param name="notify">Notify subscribers of change.</param>
    protected override void AppendCache(TOut result, bool notify)
    {
        int prevCacheCount = Cache.Count;
        
        // Call base implementation to handle cache logic
        base.AppendCache(result, notify);

        // If cache actually grew, add the pending state
        if (Cache.Count > prevCacheCount && _pendingState is not null)
        {
            StateCache.Add(_pendingState);
        }
        // If cache didn't grow, item was either duplicate or triggered rebuild
        // In rebuild case, StateCache is managed by RollbackState

        _pendingState = default;
    }

    /// <summary>
    /// Rollbacks internal state to a point in time using the StateCache.
    /// </summary>
    /// <param name="timestamp">Point in time to restore.</param>
    protected override void RollbackState(DateTime timestamp)
    {
        // Find the index in the cache that corresponds to the timestamp
        int cacheIndex = Cache.IndexGte(timestamp);

        if (cacheIndex == -1)
        {
            // Timestamp is after all cache entries, clear everything
            StateCache.Clear();
        }
        else if (cacheIndex > 0)
        {
            // Remove state entries from the rollback point onwards
            StateCache.RemoveRange(cacheIndex, StateCache.Count - cacheIndex);
        }
        else
        {
            // cacheIndex == 0, clear all state
            StateCache.Clear();
        }

        // Note: Derived classes should override this method to:
        // 1. Call base.RollbackState(timestamp) to manage StateCache
        // 2. Restore their specific state variables from the last StateCache entry
        // This eliminates the need to recalculate state from scratch
    }

    /// <summary>
    /// Prunes the cache and corresponding state to the maximum size.
    /// </summary>
    protected override void PruneCache()
    {
        if (Cache.Count < MaxCacheSize)
        {
            return;
        }

        // Synchronize state cache removal with result cache
        int itemsToRemove = Cache.Count - MaxCacheSize + 1;

        for (int i = 0; i < itemsToRemove && StateCache.Count > 0; i++)
        {
            StateCache.RemoveAt(0);
        }

        // Call base implementation to handle result cache and notifications
        base.PruneCache();
    }
}
