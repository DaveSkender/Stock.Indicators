namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for aggregating raw tick data into OHLCV quote bars.
/// </summary>
public class TickAggregatorHub
    : QuoteProvider<ITick, IQuote>
{
    private const int maxExecutionIdCacheSize = 10000;

    private readonly Dictionary<string, DateTime> _processedExecutionIds = [];
    private readonly TimeSpan _executionIdRetentionPeriod;
    private Quote? _currentBar;
    private DateTime _currentBarTimestamp;

    /// <summary>
    /// Initializes a new instance of the <see cref="TickAggregatorHub"/> class.
    /// </summary>
    /// <param name="provider">The tick data provider.</param>
    /// <param name="periodSize">The period size to aggregate to.</param>
    /// <param name="fillGaps">Whether to fill gaps by carrying forward the last known price.</param>
    public TickAggregatorHub(
        IStreamObservable<ITick> provider,
        PeriodSize periodSize,
        bool fillGaps = false)
        : base(provider)
    {
        if (periodSize == PeriodSize.Month)
        {
            throw new ArgumentException(
                $"Month aggregation is not supported in streaming mode. periodSize={periodSize}. Use TimeSpan overload for custom periods.",
                nameof(periodSize));
        }

        TimeSpan agg = periodSize.ToTimeSpan();

        if (agg == TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(periodSize), periodSize, "Unsupported PeriodSize");
        }

        AggregationPeriod = agg;
        FillGaps = fillGaps;
        Name = $"TICK-AGG({periodSize})";

        // Keep execution IDs for 100x the aggregation period or at least 1 hour
        _executionIdRetentionPeriod = TimeSpan.FromTicks(Math.Max(
            AggregationPeriod.Ticks * 100,
            TimeSpan.FromHours(1).Ticks));

        Reinitialize();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TickAggregatorHub"/> class.
    /// </summary>
    /// <param name="provider">The tick data provider.</param>
    /// <param name="timeSpan">The time span to aggregate to.</param>
    /// <param name="fillGaps">Whether to fill gaps by carrying forward the last known price.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the time span is less than or equal to zero.</exception>
    public TickAggregatorHub(
        IStreamObservable<ITick> provider,
        TimeSpan timeSpan,
        bool fillGaps = false)
        : base(provider)
    {
        if (timeSpan <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeSpan), timeSpan,
                "Aggregation period must be greater than zero.");
        }

        AggregationPeriod = timeSpan;
        FillGaps = fillGaps;
        Name = $"TICK-AGG({timeSpan})";

        // Keep execution IDs for 100x the aggregation period or at least 1 hour
        _executionIdRetentionPeriod = TimeSpan.FromTicks(Math.Max(
            timeSpan.Ticks * 100,
            TimeSpan.FromHours(1).Ticks));

        Reinitialize();
    }

    /// <summary>
    /// Gets a value indicating whether gap filling is enabled.
    /// </summary>
    public bool FillGaps { get; }

    /// <summary>
    /// Gets the aggregation period.
    /// </summary>
    public TimeSpan AggregationPeriod { get; }

    /// <inheritdoc/>
    public override void OnAdd(ITick item, bool notify, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        lock (CacheLock)
        {
            // Check for duplicate execution IDs with time-based pruning
            if (!string.IsNullOrEmpty(item.ExecutionId))
            {
                if (_processedExecutionIds.ContainsKey(item.ExecutionId))
                {
                    // Skip duplicate tick
                    return;
                }

                // Add execution ID with timestamp
                _processedExecutionIds[item.ExecutionId] = item.Timestamp;

                // Prune old execution IDs (by time or size)
                if (_processedExecutionIds.Count > (maxExecutionIdCacheSize / 2))
                {
                    DateTime pruneThreshold = item.Timestamp.Add(-_executionIdRetentionPeriod);
                    List<string> toRemove = _processedExecutionIds
                        .Where(kvp => kvp.Value < pruneThreshold)
                        .Select(kvp => kvp.Key)
                        .ToList();

                    foreach (string key in toRemove)
                    {
                        _processedExecutionIds.Remove(key);
                    }
                }

                // Hard limit: if still too large after pruning, remove oldest entries
                if (_processedExecutionIds.Count > maxExecutionIdCacheSize)
                {
                    List<string> toRemove = _processedExecutionIds
                        .OrderBy(kvp => kvp.Value)
                        .Take(_processedExecutionIds.Count - (maxExecutionIdCacheSize / 2))
                        .Select(kvp => kvp.Key)
                        .ToList();

                    foreach (string key in toRemove)
                    {
                        _processedExecutionIds.Remove(key);
                    }
                }
            }

            DateTime barTimestamp = item.Timestamp.RoundDown(AggregationPeriod);

            // Determine if this is for current bar, future bar, or past bar
            bool isFutureBar = _currentBar == null || barTimestamp > _currentBarTimestamp;
            bool isPastBar = _currentBar != null && barTimestamp < _currentBarTimestamp;

            // Handle late arrival for past bar
            if (isPastBar)
            {
                Rebuild(barTimestamp);
                return;
            }

            // Handle gap filling if enabled and moving to future bar
            if (FillGaps && isFutureBar && _currentBar != null)
            {
                DateTime lastBarTimestamp = _currentBarTimestamp;
                DateTime nextExpectedBarTimestamp = lastBarTimestamp.Add(AggregationPeriod);

                // Fill gaps between last bar and current bar
                while (nextExpectedBarTimestamp < barTimestamp)
                {
                    // Create a gap-fill bar with carried-forward prices
                    Quote gapBar = new(
                        Timestamp: nextExpectedBarTimestamp,
                        Open: _currentBar.Close,
                        High: _currentBar.Close,
                        Low: _currentBar.Close,
                        Close: _currentBar.Close,
                        Volume: 0m);

                    // Add gap bar directly to cache
                    AppendCache(gapBar, notify);

                    // Update current bar to the gap bar
                    _currentBar = gapBar;
                    _currentBarTimestamp = nextExpectedBarTimestamp;

                    nextExpectedBarTimestamp = nextExpectedBarTimestamp.Add(AggregationPeriod);
                }
            }

            // Handle new bar or update to current bar
            if (isFutureBar)
            {
                // Start a new bar from the tick
                _currentBar = CreateOrUpdateBar(null, barTimestamp, item);
                _currentBarTimestamp = barTimestamp;

                // Add the new bar directly to cache
                AppendCache(_currentBar, notify);
            }
            else // isCurrentBar
            {
                // Update existing bar with new tick data
                _currentBar = CreateOrUpdateBar(_currentBar, barTimestamp, item);

                // Replace the last item in cache with updated bar
                int index = Cache.Count - 1;
                if (index >= 0)
                {
                    Cache[index] = _currentBar;

                    // Notify observers of the update
                    if (notify)
                    {
                        NotifyObserversOnRebuild(_currentBar.Timestamp);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Creates a new bar or updates an existing bar with tick data.
    /// </summary>
    /// <param name="existingBar">Existing bar to update, or null to create new.</param>
    /// <param name="barTimestamp">Timestamp for the bar.</param>
    /// <param name="tick">Tick data to incorporate.</param>
    /// <returns>Updated or new Quote bar.</returns>
    private static Quote CreateOrUpdateBar(Quote? existingBar, DateTime barTimestamp, ITick tick)
    {
        if (existingBar == null)
        {
            // Create new bar from tick
            return new Quote(
                Timestamp: barTimestamp,
                Open: tick.Price,
                High: tick.Price,
                Low: tick.Price,
                Close: tick.Price,
                Volume: tick.Volume);
        }
        // Update existing bar
        return new Quote(
            Timestamp: barTimestamp,
            Open: existingBar.Open,
            High: Math.Max(existingBar.High, tick.Price),
            Low: Math.Min(existingBar.Low, tick.Price),
            Close: tick.Price,
            Volume: existingBar.Volume + tick.Volume);
    }

    /// <inheritdoc/>
    protected override (IQuote result, int index)
        ToIndicator(ITick item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        DateTime barTimestamp = item.Timestamp.RoundDown(AggregationPeriod);

        int index = indexHint ?? Cache.IndexGte(barTimestamp);

        if (index == -1)
        {
            index = Cache.Count;
        }

        // Convert tick to a single-price bar
        Quote bar = new(
            Timestamp: barTimestamp,
            Open: item.Price,
            High: item.Price,
            Low: item.Price,
            Close: item.Price,
            Volume: item.Volume);

        return (bar, index);
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"TICK-AGG<{AggregationPeriod}>: {Cache.Count} items";

    /// <inheritdoc/>
    protected override void RollbackState(int restoreIndex)
    {
        _currentBar = null;
        _currentBarTimestamp = default;

        if (restoreIndex < 0)
        {
            _processedExecutionIds.Clear();
            return;
        }

        // Clear execution IDs for rolled back period
        DateTime preserveTimestamp = ProviderCache[restoreIndex].Timestamp;

        List<string> toRemove = _processedExecutionIds
            .Where(kvp => kvp.Value > preserveTimestamp)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (string key in toRemove)
        {
            _processedExecutionIds.Remove(key);
        }
    }
}
