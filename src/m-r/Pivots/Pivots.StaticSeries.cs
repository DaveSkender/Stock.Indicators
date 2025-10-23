namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating pivot points series.
/// </summary>
public static partial class Pivots
{
    /// <summary>
    /// Converts a list of quotes to a list of pivot points results.
    /// </summary>
    /// <param name="quotes">The list of quotes.</param>
    /// <param name="leftSpan">The number of periods to the left of the pivot point.</param>
    /// <param name="rightSpan">The number of periods to the right of the pivot point.</param>
    /// <param name="maxTrendPeriods">The maximum number of periods for trend calculation.</param>
    /// <param name="endType">The type of end point for the pivot calculation.</param>
    /// <returns>A list of pivot points results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    public static IReadOnlyList<PivotsResult> ToPivots(
        this IReadOnlyList<IQuote> quotes,
        int leftSpan = 2,
        int rightSpan = 2,
        int maxTrendPeriods = 20,
        EndType endType = EndType.HighLow)
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(quotes);
        Validate(leftSpan, rightSpan, maxTrendPeriods);

        // initialize
        int length = quotes.Count;

        decimal?[] highLine = new decimal?[length];
        PivotTrend?[] highTrend = new PivotTrend?[length];

        decimal?[] lowLine = new decimal?[length];
        PivotTrend?[] lowTrend = new PivotTrend?[length];

        List<(decimal? highPoint, decimal? lowPoint)> fractals
           = quotes
            .ToFractal(leftSpan, rightSpan, endType)
            .Select(static f => (f.FractalBear, f.FractalBull))
            .ToList();

        int? lastHighIndex = null;
        decimal? lastHighValue = null;
        int? lastLowIndex = null;
        decimal? lastLowValue = null;

        // roll through results
        for (int i = leftSpan; i <= length - rightSpan; i++)
        {
            (decimal? highPoint, decimal? lowPoint) = fractals[i];

            // reset expired indexes
            if (lastHighIndex < i - maxTrendPeriods)
            {
                lastHighIndex = null;
                lastHighValue = null;
            }

            if (lastLowIndex < i - maxTrendPeriods)
            {
                lastLowIndex = null;
                lastLowValue = null;
            }

            // evaluate high trend
            if (highPoint != null)
            {
                // repaint trend
                if (lastHighIndex != null && highPoint != lastHighValue)
                {
                    PivotTrend trend = highPoint > lastHighValue
                        ? PivotTrend.Hh
                        : PivotTrend.Lh;

                    highLine[(int)lastHighIndex] = lastHighValue;

                    decimal? incr = (highPoint - lastHighValue)
                                 / (i - lastHighIndex);

                    for (int t = (int)lastHighIndex + 1; t <= i; t++)
                    {
                        highTrend[t] = trend;
                        highLine[t] = highPoint + (incr * (t - i));
                    }
                }

                // reset starting position
                lastHighIndex = i;
                lastHighValue = highPoint;
            }

            // evaluate low trend
            if (lowPoint != null)
            {
                // repaint trend
                if (lastLowIndex != null && lowPoint != lastLowValue)
                {
                    PivotTrend trend = lowPoint > lastLowValue
                        ? PivotTrend.Hl
                        : PivotTrend.Ll;

                    lowLine[(int)lastLowIndex] = lastLowValue;

                    decimal? incr = (lowPoint - lastLowValue)
                                 / (i - lastLowIndex);

                    for (int t = (int)lastLowIndex + 1; t <= i; t++)
                    {
                        lowTrend[t] = trend;
                        lowLine[t] = lowPoint + (incr * (t - i));
                    }
                }

                // reset starting position
                lastLowIndex = i;
                lastLowValue = lowPoint;
            }
        }

        // write results

        // TODO: this may need to be re-writes (with) for streaming
        // or even here, since it still may be better than 2 full passes

        List<PivotsResult> results = new(length);

        for (int i = 0; i < length; i++)
        {
            IQuote q = quotes[i];
            (decimal? highPoint, decimal? lowPoint) = fractals[i];

            decimal? hl = highLine[i];
            decimal? ll = lowLine[i];
            PivotTrend? ht = highTrend[i];
            PivotTrend? lt = lowTrend[i];

            results.Add(new(
                Timestamp: q.Timestamp,
                HighPoint: highPoint,
                LowPoint: lowPoint,
                HighLine: hl,
                LowLine: ll,
                HighTrend: ht,
                LowTrend: lt));
        }

        return results;
    }
}
