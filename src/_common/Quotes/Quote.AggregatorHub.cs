namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for aggregating quotes into larger time periods.
/// </summary>
public class QuoteAggregatorHub
    : QuoteProvider<IQuote, IQuote>
{
    private readonly object _addLock = new();
    private readonly Dictionary<DateTime, IQuote> _inputQuoteTracker = [];
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
                $"Month aggregation is not supported in streaming mode. periodSize={periodSize}. Use TimeSpan overload for custom periods.",
                nameof(periodSize));
        }

        AggregationPeriod = periodSize.ToTimeSpan();
        FillGaps = fillGaps;
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

        AggregationPeriod = timeSpan;
        FillGaps = fillGaps;
        Name = $"QUOTE-AGG({timeSpan})";

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
    public override void OnAdd(IQuote item, bool notify, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        lock (_addLock)
        {
            DateTime barTimestamp = item.Timestamp.RoundDown(AggregationPeriod);

            // Check if this exact input quote was already processed (duplicate detection)
            if (_inputQuoteTracker.TryGetValue(item.Timestamp, out IQuote? previousQuote))
            {
                // This is an update to a previously seen quote - need to subtract old values
                // and add new values to avoid double-counting
                if (previousQuote.Timestamp == item.Timestamp)
                {
                    // Update tracker with new quote
                    _inputQuoteTracker[item.Timestamp] = item;

                    // Rebuild from this bar to recalculate correctly
                    if (_currentBar != null && barTimestamp == _currentBarTimestamp)
                    {
                        Rebuild(barTimestamp);
                        return;
                    }
                }
            }
            else
            {
                // Track this input quote
                _inputQuoteTracker[item.Timestamp] = item;

                // Prune old tracker entries (keep last 1000 or within 10x aggregation period)
                if (_inputQuoteTracker.Count > 1000)
                {
                    DateTime pruneThreshold = item.Timestamp.Add(-10 * AggregationPeriod);
                    List<DateTime> toRemove = _inputQuoteTracker
                        .Where(kvp => kvp.Key < pruneThreshold)
                        .Select(kvp => kvp.Key)
                        .ToList();

                    foreach (DateTime key in toRemove)
                    {
                        _inputQuoteTracker.Remove(key);
                    }
                }
            }

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

                    // Add gap bar using base class logic
                    (IQuote gapResult, _) = ToIndicator(gapBar, null);
                    AppendCache(gapResult, notify);

                    // Update current bar to the gap bar
                    _currentBar = gapBar;
                    _currentBarTimestamp = nextExpectedBarTimestamp;

                    nextExpectedBarTimestamp = nextExpectedBarTimestamp.Add(AggregationPeriod);
                }
            }

            // Handle new bar or update to current bar
            if (isFutureBar)
            {
                // Start a new bar
                _currentBar = CreateOrUpdateBar(null, barTimestamp, item);
                _currentBarTimestamp = barTimestamp;

                // Use base class to add the new bar
                (IQuote result, _) = ToIndicator(_currentBar, indexHint);
                AppendCache(result, notify);
            }
            else // isCurrentBar
            {
                // Update existing bar - for quotes with same timestamp, replace
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
    /// Creates a new bar or updates an existing bar with quote data.
    /// </summary>
    /// <param name="existingBar">Existing bar to update, or null to create new.</param>
    /// <param name="barTimestamp">Timestamp for the bar.</param>
    /// <param name="quote">Quote data to incorporate.</param>
    /// <returns>Updated or new Quote bar.</returns>
    private static Quote CreateOrUpdateBar(Quote? existingBar, DateTime barTimestamp, IQuote quote)
    {
        if (existingBar == null)
        {
            // Create new bar from quote
            return new Quote(
                Timestamp: barTimestamp,
                Open: quote.Open,
                High: quote.High,
                Low: quote.Low,
                Close: quote.Close,
                Volume: quote.Volume);
        }
        else
        {
            // Update existing bar
            return new Quote(
                Timestamp: barTimestamp,
                Open: existingBar.Open,  // Keep original open
                High: Math.Max(existingBar.High, quote.High),
                Low: Math.Min(existingBar.Low, quote.Low),
                Close: quote.Close,  // Always use latest close
                Volume: existingBar.Volume + quote.Volume);
        }
    }

    /// <inheritdoc/>
    protected override (IQuote result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        DateTime barTimestamp = item.Timestamp.RoundDown(AggregationPeriod);

        int index = indexHint ?? Cache.IndexGte(barTimestamp);

        if (index == -1)
        {
            index = Cache.Count;
        }

        return (item, index);
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"QUOTE-AGG<{AggregationPeriod}>: {Cache.Count} items";

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        lock (_addLock)
        {
            _currentBar = null;
            _currentBarTimestamp = default;

            // Clear input tracker for rolled back period
            List<DateTime> toRemove = _inputQuoteTracker
                .Where(kvp => kvp.Key >= timestamp)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (DateTime key in toRemove)
            {
                _inputQuoteTracker.Remove(key);
            }
        }
    }
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
