namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for managing raw tick data.
/// </summary>
public class TickHub
    : StreamHub<ITick, ITick>, IStreamObservable<ITick>
{
    /// <summary>
    /// Indicates whether this TickHub is standalone (no external provider).
    /// </summary>
    private readonly bool _isStandalone;

    /// <summary>
    /// Initializes a new instance of the <see cref="TickHub"/> class without its own provider.
    /// </summary>
    /// <param name="maxCacheSize">Maximum in-memory cache size.</param>
    public TickHub(int? maxCacheSize = null)
        : base(new BaseProvider<ITick>())
    {
        _isStandalone = true;

        const int maxCacheSizeDefault = (int)(0.9 * int.MaxValue);

        if (maxCacheSize is not null and > maxCacheSizeDefault)
        {
            string message
                = $"'{nameof(maxCacheSize)}' must be less than {maxCacheSizeDefault}.";

            throw new ArgumentOutOfRangeException(
                nameof(maxCacheSize), maxCacheSize, message);
        }

        MaxCacheSize = maxCacheSize ?? maxCacheSizeDefault;
        Name = "TICK-HUB";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TickHub"/> class with a specified provider.
    /// </summary>
    /// <param name="provider">The tick provider.</param>
    public TickHub(
        IStreamObservable<ITick> provider)
        : base(provider ?? throw new ArgumentNullException(nameof(provider)))
    {
        ArgumentNullException.ThrowIfNull(provider);

        _isStandalone = false;
        Name = "TICK-HUB";
        Reinitialize();
    }

    /// <inheritdoc/>
    protected override (ITick result, int index)
        ToIndicator(ITick item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int index = indexHint
            ?? Cache.IndexGte(item.Timestamp);

        return (item, index == -1 ? Cache.Count : index);
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"TICKS: {Cache.Count} items";

    /// <summary>
    /// Handles adding a new tick with special handling for same-timestamp updates
    /// when TickHub is standalone (no external provider).
    /// </summary>
    /// <inheritdoc/>
    public override void OnAdd(ITick item, bool notify, int? indexHint)
    {
        // for non-standalone TickHub, use standard behavior
        if (!_isStandalone)
        {
            base.OnAdd(item, notify, indexHint);
            return;
        }

        // get result and position
        (ITick result, int index) = ToIndicator(item, indexHint);

        // check if this is a same-timestamp update (not a new item at the end)
        if (Cache.Count > 0 && index < Cache.Count && Cache[index].Timestamp == result.Timestamp)
        {
            // check if this is an exact duplicate (same values)
            // if so, defer to AppendCache for overflow tracking
            if (Cache[index].Equals(result))
            {
                AppendCache(result, notify);
                return;
            }

            // For ticks with different execution IDs but same timestamp,
            // we need to store them properly (not drop them)
            bool hasExecutionId = !string.IsNullOrEmpty(result.ExecutionId);
            bool hasCachedExecutionId = !string.IsNullOrEmpty(Cache[index].ExecutionId);

            if (hasExecutionId && hasCachedExecutionId && Cache[index].ExecutionId != result.ExecutionId)
            {
                // Different execution IDs at same timestamp - both are valid trades
                // Persist to cache before notifying observers
                Cache[index] = result;

                // Notify observers with the new tick so aggregators can process it
                if (notify)
                {
                    NotifyObserversOnAdd(result, index);
                }

                return;
            }

            // For ticks without execution IDs or same execution ID, replace in cache
            Cache[index] = result;

            // Notify appropriately based on whether it's an update or new execution
            if (notify)
            {
                if (hasExecutionId && hasCachedExecutionId && Cache[index].ExecutionId == result.ExecutionId)
                {
                    // Same execution ID - this is an update/correction
                    NotifyObserversOnRebuild(result.Timestamp);
                }
                else
                {
                    // No execution IDs - notify as addition so aggregators can process
                    NotifyObserversOnAdd(result, index);
                }
            }

            return;
        }

        // standard add behavior for new items
        AppendCache(result, notify);
    }

    /// <summary>
    /// Rebuilds the cache from a specific timestamp.
    /// For standalone TickHub, preserves cache and notifies observers.
    /// </summary>
    /// <inheritdoc/>
    public override void Rebuild(DateTime fromTimestamp)
    {
        // for standalone TickHub (no external provider),
        // we cannot rebuild from an empty provider cache
        // instead, just notify observers to rebuild from this hub's cache
        if (_isStandalone)
        {
            // rollback internal state
            RollbackState(fromTimestamp);

            // notify observers to rebuild from this hub
            NotifyObserversOnRebuild(fromTimestamp);
            return;
        }

        // standard rebuild for TickHub with external provider
        base.Rebuild(fromTimestamp);
    }
}

/// <summary>
/// Provides extension methods for aggregating tick data streams into OHLCV quote bars using a <see cref="TickAggregatorHub"/>.
/// </summary>
/// <remarks>
/// The Ticks class offers static methods to facilitate the transformation of tick data into aggregated
/// quote bars, supporting both fixed period sizes and custom time spans. These methods enable seamless integration with
/// tick data providers and allow optional gap filling to maintain continuity in price data. All members are static and
/// intended for use as extension methods on <see cref="IStreamObservable{ITick}"/> instances.
/// </remarks>
public static class Ticks
{
    /// <summary>
    /// Creates a TickAggregatorHub that aggregates ticks from the provider into OHLCV quote bars.
    /// </summary>
    /// <param name="tickProvider">The tick provider to aggregate.</param>
    /// <param name="periodSize">The period size to aggregate to.</param>
    /// <param name="fillGaps">Whether to fill gaps by carrying forward the last known price.</param>
    /// <returns>A new instance of TickAggregatorHub.</returns>
    public static TickAggregatorHub ToTickAggregatorHub(
        this IStreamObservable<ITick> tickProvider,
        PeriodSize periodSize,
        bool fillGaps = false)
        => new(tickProvider, periodSize, fillGaps);

    /// <summary>
    /// Creates a TickAggregatorHub that aggregates ticks from the provider into OHLCV quote bars.
    /// </summary>
    /// <param name="tickProvider">The tick provider to aggregate.</param>
    /// <param name="timeSpan">The time span to aggregate to.</param>
    /// <param name="fillGaps">Whether to fill gaps by carrying forward the last known price.</param>
    /// <returns>A new instance of TickAggregatorHub.</returns>
    public static TickAggregatorHub ToTickAggregatorHub(
        this IStreamObservable<ITick> tickProvider,
        TimeSpan timeSpan,
        bool fillGaps = false)
        => new(tickProvider, timeSpan, fillGaps);
}
