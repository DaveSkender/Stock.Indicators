namespace Skender.Stock.Indicators;

// PIVOTS (SERIES)

public static partial class Indicator
{
    internal static List<PivotsResult> CalcPivots<TQuote>(
        this List<TQuote> quotesList,
        int leftSpan,
        int rightSpan,
        int maxTrendPeriods,
        EndType endType)
        where TQuote : IQuote
    {
        // check parameter arguments
        Pivots.Validate(leftSpan, rightSpan, maxTrendPeriods);

        // initialize

        List<PivotsResult> results
           = quotesList
            .CalcFractal(leftSpan, rightSpan, endType)
            .Select(x => new PivotsResult()
            {
                TickDate = x.TickDate,
                HighPoint = x.FractalBear,
                LowPoint = x.FractalBull
            })
            .ToList();

        int? lastHighIndex = null;
        decimal? lastHighValue = null;
        int? lastLowIndex = null;
        decimal? lastLowValue = null;

        // roll through results
        for (int i = leftSpan; i <= results.Count - rightSpan; i++)
        {
            PivotsResult r = results[i];

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
            if (r.HighPoint != null)
            {
                // repaint trend
                if (lastHighIndex != null && r.HighPoint != lastHighValue)
                {
                    PivotTrend trend = (r.HighPoint > lastHighValue)
                        ? PivotTrend.HH
                        : PivotTrend.LH;

                    results[(int)lastHighIndex].HighLine = lastHighValue;

                    decimal? incr = (r.HighPoint - lastHighValue)
                                 / (i - lastHighIndex);

                    for (int t = (int)lastHighIndex + 1; t <= i; t++)
                    {
                        results[t].HighTrend = trend;
                        results[t].HighLine = r.HighPoint + (incr * (t - i));
                    }
                }

                // reset starting position
                lastHighIndex = i;
                lastHighValue = r.HighPoint;
            }

            // evaluate low trend
            if (r.LowPoint != null)
            {
                // repaint trend
                if (lastLowIndex != null && r.LowPoint != lastLowValue)
                {
                    PivotTrend trend = (r.LowPoint > lastLowValue)
                        ? PivotTrend.HL
                        : PivotTrend.LL;

                    results[(int)lastLowIndex].LowLine = lastLowValue;

                    decimal? incr = (r.LowPoint - lastLowValue)
                                 / (i - lastLowIndex);

                    for (int t = (int)lastLowIndex + 1; t <= i; t++)
                    {
                        results[t].LowTrend = trend;
                        results[t].LowLine = r.LowPoint + (incr * (t - i));
                    }
                }

                // reset starting position
                lastLowIndex = i;
                lastLowValue = r.LowPoint;
            }
        }

        return results;
    }
}
