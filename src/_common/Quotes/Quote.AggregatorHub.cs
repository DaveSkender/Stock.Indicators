namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for aggregating quotes into larger time periods.
/// </summary>
public class QuoteAggregatorHub
    : QuoteProvider<IQuote, IQuote>
{
    private readonly TimeSpan _aggregationPeriod;
    private readonly bool _fillGaps;
    private Quote? _currentBar;
    private DateTime _currentBarTimestamp;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuoteAggregatorHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="periodSize">The period size to aggregate to.</param>
    /// <param name="fillGaps">Whether to fill gaps by carrying forward the last known price.</param>
    public QuoteAggregatorHub(
        IQuoteProvider<IQuote> provider,
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

        _aggregationPeriod = periodSize.ToTimeSpan();
        _fillGaps = fillGaps;
        Name = $"QUOTE-AGG({periodSize})";

        Reinitialize();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QuoteAggregatorHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="timeSpan">The time span to aggregate to.</param>
    /// <param name="fillGaps">Whether to fill gaps by carrying forward the last known price.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the time span is less than or equal to zero.</exception>
    public QuoteAggregatorHub(
        IQuoteProvider<IQuote> provider,
        TimeSpan timeSpan,
        bool fillGaps = false)
        : base(provider)
    {
        if (timeSpan <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeSpan), timeSpan,
                "Aggregation period must be greater than zero.");
        }

        _aggregationPeriod = timeSpan;
        _fillGaps = fillGaps;
        Name = $"QUOTE-AGG({timeSpan})";

        Reinitialize();
    }

    /// <summary>
    /// Gets a value indicating whether gap filling is enabled.
    /// </summary>
    public bool FillGaps => _fillGaps;

    /// <summary>
    /// Gets the aggregation period.
    /// </summary>
    public TimeSpan AggregationPeriod => _aggregationPeriod;

    /// <inheritdoc/>
    public override void OnAdd(IQuote item, bool notify, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        DateTime barTimestamp = item.Timestamp.RoundDown(_aggregationPeriod);

        // Handle gap filling if enabled
        if (_fillGaps && _currentBar != null)
        {
            DateTime lastBarTimestamp = _currentBarTimestamp;
            DateTime nextExpectedBarTimestamp = lastBarTimestamp.Add(_aggregationPeriod);

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

                // Add gap bar using base class logic
                (IQuote gapResult, int gapIndex) = ToIndicator(gapBar, null);
                AppendCache(gapResult, notify);

                // Update current bar to the gap bar
                _currentBar = gapBar;
                _currentBarTimestamp = nextExpectedBarTimestamp;

                nextExpectedBarTimestamp = nextExpectedBarTimestamp.Add(_aggregationPeriod);
            }
        }

        // Check if this is a new bar or continuation of current bar
        if (_currentBar == null || barTimestamp != _currentBarTimestamp)
        {
            // Start a new bar
            _currentBar = new Quote(
                Timestamp: barTimestamp,
                Open: item.Open,
                High: item.High,
                Low: item.Low,
                Close: item.Close,
                Volume: item.Volume);

            _currentBarTimestamp = barTimestamp;

            // Use base class to add the new bar
            (IQuote result, int index) = ToIndicator(_currentBar, indexHint);
            AppendCache(result, notify);
        }
        else
        {
            // Update existing bar
            _currentBar = new Quote(
                Timestamp: barTimestamp,
                Open: _currentBar.Open,  // Keep original open
                High: Math.Max(_currentBar.High, item.High),
                Low: Math.Min(_currentBar.Low, item.Low),
                Close: item.Close,  // Always use latest close
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
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        DateTime barTimestamp = item.Timestamp.RoundDown(_aggregationPeriod);

        int index = indexHint ?? Cache.IndexGte(barTimestamp);

        if (index == -1)
        {
            index = Cache.Count;
        }

        return (item, index);
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"QUOTE-AGG<{_aggregationPeriod}>: {Cache.Count} items";
}

public static partial class Quotes
{
    /// <summary>
    /// Creates a QuoteAggregatorHub that aggregates quotes from the provider into larger time periods.
    /// </summary>
    /// <param name="quoteProvider">The quote provider to aggregate.</param>
    /// <param name="periodSize">The period size to aggregate to.</param>
    /// <param name="fillGaps">Whether to fill gaps by carrying forward the last known price.</param>
    /// <returns>A new instance of QuoteAggregatorHub.</returns>
    public static QuoteAggregatorHub ToQuoteAggregatorHub(
        this IQuoteProvider<IQuote> quoteProvider,
        PeriodSize periodSize,
        bool fillGaps = false)
        => new(quoteProvider, periodSize, fillGaps);

    /// <summary>
    /// Creates a QuoteAggregatorHub that aggregates quotes from the provider into larger time periods.
    /// </summary>
    /// <param name="quoteProvider">The quote provider to aggregate.</param>
    /// <param name="timeSpan">The time span to aggregate to.</param>
    /// <param name="fillGaps">Whether to fill gaps by carrying forward the last known price.</param>
    /// <returns>A new instance of QuoteAggregatorHub.</returns>
    public static QuoteAggregatorHub ToQuoteAggregatorHub(
        this IQuoteProvider<IQuote> quoteProvider,
        TimeSpan timeSpan,
        bool fillGaps = false)
        => new(quoteProvider, timeSpan, fillGaps);
}
