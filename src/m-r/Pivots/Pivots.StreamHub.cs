namespace Skender.Stock.Indicators;

// PIVOT POINTS (STREAM HUB)

/// <summary>
/// Provides methods for calculating Pivot Points using a stream hub.
/// </summary>
public class PivotsHub
    : StreamHub<IQuote, PivotsResult>, IPivots
{
    #region fields

    private readonly string hubName;

    #endregion fields

    #region constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PivotsHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="leftSpan">The number of periods to the left of the pivot point.</param>
    /// <param name="rightSpan">The number of periods to the right of the pivot point.</param>
    /// <param name="maxTrendPeriods">The maximum number of periods for trend calculation.</param>
    /// <param name="endType">The type of end point for the pivot calculation.</param>
    internal PivotsHub(
        IQuoteProvider<IQuote> provider,
        int leftSpan,
        int rightSpan,
        int maxTrendPeriods,
        EndType endType) : base(provider)
    {
        Pivots.Validate(leftSpan, rightSpan, maxTrendPeriods);

        LeftSpan = leftSpan;
        RightSpan = rightSpan;
        MaxTrendPeriods = maxTrendPeriods;
        EndType = endType;
        hubName = $"PIVOTS({leftSpan},{rightSpan},{maxTrendPeriods},{endType.ToString().ToUpperInvariant()})";

        Reinitialize();
    }

    #endregion constructors

    #region properties

    /// <summary>
    /// Gets the number of periods to the left for pivot identification.
    /// </summary>
    public int LeftSpan { get; init; }

    /// <summary>
    /// Gets the number of periods to the right for pivot identification.
    /// </summary>
    public int RightSpan { get; init; }

    /// <summary>
    /// Gets the maximum number of periods to track trend.
    /// </summary>
    public int MaxTrendPeriods { get; init; }

    /// <summary>
    /// Gets the end type for price calculations.
    /// </summary>
    public EndType EndType { get; init; }

    #endregion properties

    #region methods

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <summary>
    /// Rebuilds the hub from the beginning, including trend line calculations.
    /// </summary>
    /// <param name="fromTimestamp">The timestamp to rebuild from.</param>
    public new void Rebuild(DateTime fromTimestamp)
    {
        // First, rebuild pivot points
        base.Rebuild(fromTimestamp);

        // Then calculate trend lines by making a second pass
        CalculateTrendLines();
    }

    /// <summary>
    /// Rebuilds the hub from a specific index, including trend line calculations.
    /// </summary>
    /// <param name="fromIndex">The index to rebuild from.</param>
    public new void Rebuild(int fromIndex)
    {
        // First, rebuild pivot points
        base.Rebuild(fromIndex);

        // Then calculate trend lines by making a second pass
        CalculateTrendLines();
    }

    private void CalculateTrendLines()
    {
        // Calculate trend lines similar to Series implementation
        int? lastHighIndex = null;
        decimal? lastHighValue = null;
        int? lastLowIndex = null;
        decimal? lastLowValue = null;

        for (int i = LeftSpan; i <= Cache.Count - RightSpan; i++)
        {
            if (i >= Cache.Count)
            {
                break;
            }

            PivotsResult current = Cache[i];

            // Reset expired indexes
            if (lastHighIndex < i - MaxTrendPeriods)
            {
                lastHighIndex = null;
                lastHighValue = null;
            }

            if (lastLowIndex < i - MaxTrendPeriods)
            {
                lastLowIndex = null;
                lastLowValue = null;
            }

            // Evaluate high trend
            if (current.HighPoint != null)
            {
                // Repaint trend
                if (lastHighIndex != null && current.HighPoint != lastHighValue)
                {
                    PivotTrend trend = current.HighPoint > lastHighValue
                        ? PivotTrend.Hh
                        : PivotTrend.Lh;

                    // Update the last high result with its line value
                    Cache[(int)lastHighIndex] = Cache[(int)lastHighIndex] with {
                        HighLine = lastHighValue
                    };

                    decimal? incr = (current.HighPoint - lastHighValue)
                        / (i - lastHighIndex);

                    // Repaint trend line from last high + 1 to current high
                    for (int t = (int)lastHighIndex + 1; t <= i; t++)
                    {
                        if (t < Cache.Count)
                        {
                            Cache[t] = Cache[t] with {
                                HighLine = current.HighPoint + (incr * (t - i)),
                                HighTrend = trend
                            };
                        }
                    }
                }

                // Reset starting position
                lastHighIndex = i;
                lastHighValue = current.HighPoint;
            }

            // Evaluate low trend
            if (current.LowPoint != null)
            {
                // Repaint trend
                if (lastLowIndex != null && current.LowPoint != lastLowValue)
                {
                    PivotTrend trend = current.LowPoint > lastLowValue
                        ? PivotTrend.Hl
                        : PivotTrend.Ll;

                    // Update the last low result with its line value
                    Cache[(int)lastLowIndex] = Cache[(int)lastLowIndex] with {
                        LowLine = lastLowValue
                    };

                    decimal? incr = (current.LowPoint - lastLowValue)
                        / (i - lastLowIndex);

                    // Repaint trend line from last low + 1 to current low
                    for (int t = (int)lastLowIndex + 1; t <= i; t++)
                    {
                        if (t < Cache.Count)
                        {
                            Cache[t] = Cache[t] with {
                                LowLine = current.LowPoint + (incr * (t - i)),
                                LowTrend = trend
                            };
                        }
                    }
                }

                // Reset starting position
                lastLowIndex = i;
                lastLowValue = current.LowPoint;
            }
        }
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // No state to rollback for Pivots streaming implementation
        // Pivot points are calculated independently for each position
        // Trend lines require Rebuild() to calculate with full history
    }

    /// <inheritdoc/>
    protected override (PivotsResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);
        int length = ProviderCache.Count;

        // Initialize result with no values
        decimal? highPoint = null;
        decimal? lowPoint = null;

        // Can we calculate fractal for the quote at index i?
        // We need LeftSpan quotes before it and RightSpan quotes after it
        if (i + 1 > LeftSpan && i + 1 <= length - RightSpan)
        {
            IQuote center = ProviderCache[i];
            bool isHigh = true;
            bool isLow = true;

            decimal evalHigh = EndType == EndType.Close ? center.Close : center.High;
            decimal evalLow = EndType == EndType.Close ? center.Close : center.Low;

            // Compare center with wings (both left and right)
            for (int p = i - LeftSpan; p <= i + RightSpan; p++)
            {
                // Skip center
                if (p == i)
                {
                    continue;
                }

                IQuote wing = ProviderCache[p];
                decimal wingHigh = EndType == EndType.Close ? wing.Close : wing.High;
                decimal wingLow = EndType == EndType.Close ? wing.Close : wing.Low;

                if (evalHigh <= wingHigh)
                {
                    isHigh = false;
                }

                if (evalLow >= wingLow)
                {
                    isLow = false;
                }
            }

            highPoint = isHigh ? evalHigh : null;
            lowPoint = isLow ? evalLow : null;
        }

        // Note: Trend lines (HighLine, LowLine, HighTrend, LowTrend) require
        // repainting historical values which is not compatible with streaming architecture.
        // These values are calculated during Rebuild() to match Series implementation.
        // In real-time streaming mode, only pivot points are identified.

        // Create result for the current quote with only pivot points
        PivotsResult result = new(
            Timestamp: ProviderCache[i].Timestamp,
            HighPoint: highPoint,
            LowPoint: lowPoint,
            HighLine: null,
            LowLine: null,
            HighTrend: null,
            LowTrend: null);

        return (result, i);
    }

    #endregion methods
}


public static partial class Pivots
{
    /// <summary>
    /// Creates a Pivots streaming hub from a quote provider.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="leftSpan">The number of periods to the left of the pivot point. Default is 2.</param>
    /// <param name="rightSpan">The number of periods to the right of the pivot point. Default is 2.</param>
    /// <param name="maxTrendPeriods">The maximum number of periods for trend calculation. Default is 20.</param>
    /// <param name="endType">The type of end point for the pivot calculation. Default is <see cref="EndType.HighLow"/>.</param>
    /// <returns>An instance of <see cref="PivotsHub"/>.</returns>
    public static PivotsHub ToPivotsHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int leftSpan = 2,
        int rightSpan = 2,
        int maxTrendPeriods = 20,
        EndType endType = EndType.HighLow)
    {
        ArgumentNullException.ThrowIfNull(quoteProvider);
        return new(quoteProvider, leftSpan, rightSpan, maxTrendPeriods, endType);
    }

    /// <summary>
    /// Creates a Pivots hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="leftSpan">The number of periods to the left of the pivot point. Default is 2.</param>
    /// <param name="rightSpan">The number of periods to the right of the pivot point. Default is 2.</param>
    /// <param name="maxTrendPeriods">The maximum number of periods for trend calculation. Default is 20.</param>
    /// <param name="endType">The type of end point for the pivot calculation. Default is <see cref="EndType.HighLow"/>.</param>
    /// <returns>An instance of <see cref="PivotsHub"/>.</returns>
    public static PivotsHub ToPivotsHub(
        this IReadOnlyList<IQuote> quotes,
        int leftSpan = 2,
        int rightSpan = 2,
        int maxTrendPeriods = 20,
        EndType endType = EndType.HighLow)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToPivotsHub(leftSpan, rightSpan, maxTrendPeriods, endType);
    }
}
