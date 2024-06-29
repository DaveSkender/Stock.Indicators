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
        int length = quotesList.Count;

        decimal?[] highLine = new decimal?[length];
        PivotTrend?[] highTrend = new PivotTrend?[length];

        decimal?[] lowLine = new decimal?[length];
        PivotTrend?[] lowTrend = new PivotTrend?[length];

        List<(decimal? highPoint, decimal? lowPoint)> fractals
           = quotesList
            .CalcFractal(leftSpan, rightSpan, endType)
            .Select(f => (f.FractalBear, f.FractalBull))
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
                    PivotTrend trend = (highPoint > lastHighValue)
                        ? PivotTrend.HH
                        : PivotTrend.LH;

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
                    PivotTrend trend = (lowPoint > lastLowValue)
                        ? PivotTrend.HL
                        : PivotTrend.LL;

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
            TQuote q = quotesList[i];
            (decimal? highPoint, decimal? lowPoint) = fractals[i];

            decimal? hl = highLine[i];
            decimal? ll = lowLine[i];
            PivotTrend? ht = highTrend[i];
            PivotTrend? lt = lowTrend[i];

            results.Add(new PivotsResult(
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
