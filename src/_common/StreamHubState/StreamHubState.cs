namespace Skender.Stock.Indicators;

#pragma warning disable CA1062 // Validate arguments of public methods

// STREAM HUB (STATE MANAGEMENT)
/// <summary>
/// StreamHub with state management for stateful indicators.
/// Optimized for rapid updates to the latest candle with optional state caching.
/// </summary>
/// <typeparam name="TIn">Type of inbound provider data.</typeparam>
/// <typeparam name="TState">Type of state object for inter-candle computation state.</typeparam>
/// <typeparam name="TOut">Type of outbound indicator data.</typeparam>
public abstract class StreamHubState<TIn, TState, TOut> : StreamHub<TIn, TOut>
    where TIn : IReusable
    where TOut : IReusable
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
    /// Cached state from the previous bar (one bar ago).
    /// Null when cache is invalid or no previous state exists.
    /// </summary>
    private TState? _cachedPreviousState;

    /// <summary>
    /// Timestamp of the last processed item to detect same-candle updates.
    /// </summary>
    private DateTime? _lastTimestamp;

    /// <summary>
    /// The last timestamp where we successfully cached state.
    /// Used to detect when we've moved past a rebuild point.
    /// </summary>
    private DateTime? _lastCachedTimestamp;

    /// <summary>
    /// Converts incremental value into an indicator candidate, state, and cache position.
    /// </summary>
    /// <param name="item">New item from provider.</param>
    /// <param name="indexHint">Provider index hint.</param>
    /// <returns>Cacheable item candidate, state, and index hint.</returns>
    protected abstract (TOut result, TState state, int index) ToIndicatorState(TIn item, int? indexHint);

    /// <summary>
    /// Restores indicator state from the previous cached state.
    /// Called when updating the same timestamp to avoid full recalculation.
    /// If previousState is null, implementations should reset to initial state.
    /// </summary>
    /// <param name="previousState">The cached state from one bar ago, or null to reset.</param>
    protected abstract void RestorePreviousState(TState? previousState);

    /// <summary>
    /// Converts incremental value into an indicator candidate and cache position.
    /// This implementation calls ToIndicatorState and manages state caching.
    /// </summary>
    /// <param name="item">New item from provider.</param>
    /// <param name="indexHint">Provider index hint.</param>
    /// <returns>Cacheable item candidate and index hint.</returns>
    protected sealed override (TOut result, int index) ToIndicator(TIn item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        // Check if this is an update to the same candle (rapid update scenario)
        bool isSameCandleUpdate = _lastTimestamp.HasValue
            && item.Timestamp == _lastTimestamp.Value
            && _cachedPreviousState is not null;

        // If updating same candle and we have cached state, restore it for fast path
        if (isSameCandleUpdate)
        {
            RestorePreviousState(_cachedPreviousState);
        }

        // Call the state-aware method
        (TOut result, TState state, int index) = ToIndicatorState(item, indexHint);

        // Update cache for next iteration
        // Cache current state as "previous" for potential same-candle updates
        // Only cache if this is genuinely new data (not a rebuild/replay)
        bool isNewTimestamp = !_lastCachedTimestamp.HasValue || item.Timestamp > _lastCachedTimestamp.Value;

        if (isNewTimestamp && item.Timestamp != _lastTimestamp)
        {
            // New candle with never-seen-before timestamp - safe to cache
            _cachedPreviousState = state;
            _lastTimestamp = item.Timestamp;
            _lastCachedTimestamp = item.Timestamp;
        }
        else if (isNewTimestamp && item.Timestamp == _lastTimestamp)
        {
            // Same timestamp but new data (rapid update) - update last cached timestamp
            _lastCachedTimestamp = item.Timestamp;
        }
        // If not a new timestamp, we're in a rebuild - don't cache

        return (result, index);
    }

    /// <summary>
    /// Adds an item to cache. Invalidates state cache on any complex operation.
    /// </summary>
    /// <param name="result">Item to cache.</param>
    /// <param name="notify">Notify subscribers of change.</param>
    protected override void AppendCache(TOut result, bool notify)
    {
        int prevCacheCount = Cache.Count;

        // Check if this will be a complex operation (rebuild needed)
        // If cache will NOT grow, a complex operation will occur
        bool willRebuild = Cache.Count > 0 && result.Timestamp <= Cache[^1].Timestamp;

        if (willRebuild)
        {
            // Invalidate cache BEFORE the rebuild happens
            InvalidateStateCache();
        }

        // Call base implementation to handle cache logic (may trigger rebuild)
        base.AppendCache(result, notify);
    }

    /// <summary>
    /// Rollbacks internal state to a point in time.
    /// For same-timestamp rollback (updating latest candle), uses fast path with cached state.
    /// Otherwise, invalidates cache and reverts to stateless behavior.
    /// </summary>
    /// <param name="timestamp">Point in time to restore.</param>
    protected override void RollbackState(DateTime timestamp)
    {
        // Find the index in the cache that corresponds to the timestamp
        int cacheIndex = Cache.IndexGte(timestamp);

        if (cacheIndex == -1)
        {
            // Timestamp is after all cache entries, no rollback needed
            return;
        }

        // Check if this is a same-candle rollback (updating latest candle)
        bool isSameCandleRollback = cacheIndex == Cache.Count - 1
            && Cache.Count > 0
            && Cache[^1].Timestamp == timestamp
            && _cachedPreviousState is not null;

        if (isSameCandleRollback)
        {
            // Fast path: updating latest candle, restore from cached previous state
            RestorePreviousState(_cachedPreviousState);
        }
        else
        {
            // Complex rollback (insert/remove) - invalidate cache and reset state
            // This ensures the indicator recalculates from scratch during rebuild
            InvalidateStateCache();
            RestorePreviousState(default);
        }

        // Note: Derived classes can override this method to add additional cleanup
        // but should call base.RollbackState(timestamp) to maintain proper cache management
    }

    /// <summary>
    /// Invalidates the cached state, reverting to stateless behavior.
    /// Called when complex operations (insert, remove, rebuild) occur.
    /// </summary>
    private void InvalidateStateCache()
    {
        _cachedPreviousState = default;
        _lastTimestamp = null;
        // Don't reset _lastCachedTimestamp - it tracks historical max timestamp
    }

    /// <summary>
    /// Prunes the cache. Invalidates state cache as pruning is a complex operation.
    /// </summary>
    protected override void PruneCache()
    {
        if (Cache.Count < MaxCacheSize)
        {
            return;
        }

        // Pruning is a complex operation - invalidate cached state
        InvalidateStateCache();

        // Call base implementation to handle result cache and notifications
        base.PruneCache();
    }
}
