namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for aggregating raw tick data into OHLCV quote bars.
/// </summary>
public class TickAggregatorHub
    : QuoteProvider<ITick, IQuote>
{
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

        DateTime barTimestamp = item.Timestamp.RoundDown(AggregationPeriod);

        // Handle gap filling if enabled
        if (FillGaps && _currentBar != null)
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

        // Check if this is a new bar or continuation of current bar
        if (_currentBar == null || barTimestamp != _currentBarTimestamp)
        {
            // Start a new bar from the tick
            _currentBar = new Quote(
                Timestamp: barTimestamp,
                Open: item.Price,
                High: item.Price,
                Low: item.Price,
                Close: item.Price,
                Volume: item.Volume);

            _currentBarTimestamp = barTimestamp;

            // Add the new bar directly to cache
            AppendCache(_currentBar, notify);
        }
        else
        {
            // Update existing bar with new tick data
            _currentBar = new Quote(
                Timestamp: barTimestamp,
                Open: _currentBar.Open,  // Keep original open
                High: Math.Max(_currentBar.High, item.Price),
                Low: Math.Min(_currentBar.Low, item.Price),
                Close: item.Price,  // Always use latest price
                Volume: _currentBar.Volume + item.Volume);

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
