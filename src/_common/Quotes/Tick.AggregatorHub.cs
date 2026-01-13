namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for aggregating raw tick data into OHLCV quote bars.
/// </summary>
public class TickAggregatorHub
    : QuoteProvider<ITick, IQuote>
{
    private Quote? _currentBar;
    private DateTime _currentBarTimestamp;
    private readonly HashSet<string> _processedExecutionIds = [];

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
                "Month aggregation is not supported in streaming mode. Use TimeSpan overload for custom periods.",
                nameof(periodSize));
        }

        AggregationPeriod = periodSize.ToTimeSpan();
        FillGaps = fillGaps;
        Name = $"TICK-AGG({periodSize})";

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

        // Check for duplicate execution IDs
        if (!string.IsNullOrEmpty(item.ExecutionId))
        {
            if (_processedExecutionIds.Contains(item.ExecutionId))
            {
                return; // Skip duplicate tick
            }

            _processedExecutionIds.Add(item.ExecutionId);
        }

        DateTime barTimestamp = item.Timestamp.RoundDown(AggregationPeriod);

        // Determine if this is for current bar, future bar, or past bar
        bool isCurrentBar = _currentBar != null && barTimestamp == _currentBarTimestamp;
        bool isFutureBar = _currentBar == null || barTimestamp > _currentBarTimestamp;
        bool isPastBar = _currentBar != null && barTimestamp < _currentBarTimestamp;

        // Handle late arrival for past bar
        if (isPastBar)
        {
            // Find the existing bar in cache
            int existingIndex = Cache.IndexGte(barTimestamp);
            if (existingIndex >= 0 && existingIndex < Cache.Count && Cache[existingIndex].Timestamp == barTimestamp)
            {
                // Update existing past bar
                IQuote existingBar = Cache[existingIndex];
                Quote updatedBar = new(
                    Timestamp: barTimestamp,
                    Open: existingBar.Open,  // Keep original open
                    High: Math.Max(existingBar.High, item.Price),
                    Low: Math.Min(existingBar.Low, item.Price),
                    Close: item.Price,  // Update close
                    Volume: existingBar.Volume + item.Volume);

                Cache[existingIndex] = updatedBar;

                // Trigger rebuild from this timestamp
                if (notify)
                {
                    NotifyObserversOnRebuild(barTimestamp);
                }
            }
            return;
        }

        // Handle gap filling if enabled and moving to future bar
        if (FillGaps && isFutureBar && _currentBar != null)
        {
            FillGapsBetween(_currentBarTimestamp, barTimestamp, notify);
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

    /// <summary>
    /// Fills gaps between two timestamps by creating gap-fill bars with carried-forward prices.
    /// Only fills gaps when FillGaps is enabled and _currentBar is not null.
    /// </summary>
    /// <param name="fromTimestamp">Start timestamp (exclusive).</param>
    /// <param name="toTimestamp">End timestamp (exclusive).</param>
    /// <param name="notify">Whether to notify observers when adding gap bars.</param>
    private void FillGapsBetween(DateTime fromTimestamp, DateTime toTimestamp, bool notify)
    {
        if (!FillGaps || _currentBar == null)
        {
            return;
        }

        DateTime nextExpectedBarTimestamp = fromTimestamp.Add(AggregationPeriod);

        // Fill gaps between fromTimestamp and toTimestamp
        while (nextExpectedBarTimestamp < toTimestamp)
        {
            // Create a gap-fill bar with carried-forward prices
            Quote gapBar = new(
                Timestamp: nextExpectedBarTimestamp,
                Open: _currentBar.Close,
                High: _currentBar.Close,
                Low: _currentBar.Close,
                Close: _currentBar.Close,
                Volume: 0m);

            if (notify)
            {
                // Add gap bar using base class logic with notifications
                (IQuote gapResult, int gapIndex) = ToIndicator(
                    new Tick(nextExpectedBarTimestamp, _currentBar.Close, 0m), null);
                AppendCache(gapBar, notify);
            }
            else
            {
                // Add to cache directly (we're in rebuild context, no notifications)
                Cache.Add(gapBar);
            }

            // Update current bar to the gap bar
            _currentBar = gapBar;
            _currentBarTimestamp = nextExpectedBarTimestamp;

            nextExpectedBarTimestamp = nextExpectedBarTimestamp.Add(AggregationPeriod);
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
        else
        {
            // Update existing bar
            return new Quote(
                Timestamp: barTimestamp,
                Open: existingBar.Open,  // Keep original open
                High: Math.Max(existingBar.High, tick.Price),
                Low: Math.Min(existingBar.Low, tick.Price),
                Close: tick.Price,  // Always use latest price
                Volume: existingBar.Volume + tick.Volume);
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Override Rebuild to handle aggregator-specific rebuild logic.
    /// When rebuilding, we need to allow re-processing of provider ticks that were already
    /// aggregated, which would normally be blocked by duplicate detection.
    /// </remarks>
    public override void Rebuild(DateTime fromTimestamp)
    {
        // Round down to our aggregation period
        DateTime aggregatedTimestamp = fromTimestamp.RoundDown(AggregationPeriod);

        // Clear cache from the aggregated timestamp onwards
        RemoveRange(aggregatedTimestamp, notify: false);

        // Explicitly reset state after RemoveRange to ensure consistency
        // This ensures _currentBar/_currentBarTimestamp reflect what remains in cache
        RollbackState(aggregatedTimestamp);

        // Get provider position for the aggregated timestamp
        int provIndex = ProviderCache.IndexGte(aggregatedTimestamp);

        // Rebuild from provider cache
        if (provIndex >= 0)
        {
            int cacheSize = ProviderCache.Count;
            for (int i = provIndex; i < cacheSize; i++)
            {
                ITick providerTick = ProviderCache[i];
                DateTime barTimestamp = providerTick.Timestamp.RoundDown(AggregationPeriod);

                // Skip if this provider tick belongs to a period before our aggregated timestamp
                if (barTimestamp < aggregatedTimestamp)
                {
                    continue;
                }

                // Process the provider tick
                // We call our own aggregation logic directly instead of OnAdd
                // to avoid duplicate detection issues
                bool isCurrentBar = _currentBar != null && barTimestamp == _currentBarTimestamp;
                bool isFutureBar = _currentBar == null || barTimestamp > _currentBarTimestamp;

                // Apply gap-filling if transitioning to future bar
                if (isFutureBar && _currentBar != null)
                {
                    FillGapsBetween(_currentBarTimestamp, barTimestamp, notify: false);
                }

                if (isFutureBar)
                {
                    // Start a new aggregated bar
                    _currentBar = CreateOrUpdateBar(null, barTimestamp, providerTick);
                    _currentBarTimestamp = barTimestamp;

                    // Add to cache
                    Cache.Add(_currentBar);
                }
                else if (isCurrentBar)
                {
                    // Update existing aggregated bar
                    _currentBar = CreateOrUpdateBar(_currentBar, barTimestamp, providerTick);

                    // Replace in cache
                    int cacheIndex = Cache.Count - 1;
                    if (cacheIndex >= 0)
                    {
                        Cache[cacheIndex] = _currentBar;
                    }
                }
            }
        }

        // Notify observers with the aggregated timestamp
        NotifyObserversOnRebuild(aggregatedTimestamp);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Reset current bar state when rolling back to a specific timestamp.
    /// This is needed during cache rebuilds to ensure we don't maintain stale state.
    /// </remarks>
    protected override void RollbackState(DateTime timestamp)
    {
        // Round down to our aggregation period
        DateTime aggregatedTimestamp = timestamp.RoundDown(AggregationPeriod);

        // Find the last bar before the rollback timestamp
        // We need to search backwards from the cache
        Quote? lastBarBeforeRollback = null;
        for (int i = Cache.Count - 1; i >= 0; i--)
        {
            if (Cache[i].Timestamp < aggregatedTimestamp)
            {
                lastBarBeforeRollback = new Quote(
                    Timestamp: Cache[i].Timestamp,
                    Open: Cache[i].Open,
                    High: Cache[i].High,
                    Low: Cache[i].Low,
                    Close: Cache[i].Close,
                    Volume: Cache[i].Volume);
                break;
            }
        }

        if (lastBarBeforeRollback is not null)
        {
            // Set current bar to the last bar before the rollback timestamp
            _currentBar = lastBarBeforeRollback;
            _currentBarTimestamp = _currentBar.Timestamp;
        }
        else
        {
            // No bars before rollback timestamp - reset to null
            _currentBar = null;
            _currentBarTimestamp = DateTime.MinValue;
        }

        // Clear processed execution IDs to allow re-processing during rebuild
        _processedExecutionIds.Clear();
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
}
