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
        : base(provider)
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

            // replace existing item at this position (different values, same timestamp)
            Cache[index] = result;

            // notify observers to rebuild from this timestamp
            if (notify)
            {
                NotifyObserversOnRebuild(result.Timestamp);
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

public static partial class Ticks
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
