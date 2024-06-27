namespace Skender.Stock.Indicators;

// SLOPE AND LINEAR REGRESSION (SERIES)

public static partial class Indicator
{
    // calculate series
    internal static List<SlopeResult> CalcSlope<T>(
        this List<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Slope.Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<SlopeResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            var s = source[i];

            SlopeResult r = new() { Timestamp = s.Timestamp };
            results.Add(r);

            // skip initialization period
            if (i + 1 < lookbackPeriods)
            {
                continue;
            }

            // get averages for period
            double sumX = 0;
            double sumY = 0;

            for (int p = i - lookbackPeriods + 1; p <= i; p++)
            {
                var ps = source[p];

                sumX += p + 1d;
                sumY += ps.Value;
            }

            double avgX = sumX / lookbackPeriods;
            double avgY = sumY / lookbackPeriods;

            // least squares method
            double sumSqX = 0;
            double sumSqY = 0;
            double sumSqXY = 0;

            for (int p = i - lookbackPeriods + 1; p <= i; p++)
            {
                var ps = source[p];

                double devX = p + 1d - avgX;
                double devY = ps.Value - avgY;

                sumSqX += devX * devX;
                sumSqY += devY * devY;
                sumSqXY += devX * devY;
            }

            r.Slope = (sumSqXY / sumSqX).NaN2Null();
            r.Intercept = (avgY - (r.Slope * avgX)).NaN2Null();

            // calculate Standard Deviation and R-Squared
            double stdDevX = Math.Sqrt(sumSqX / lookbackPeriods);
            double stdDevY = Math.Sqrt(sumSqY / lookbackPeriods);
            r.StdDev = stdDevY.NaN2Null();

            if (stdDevX * stdDevY != 0)
            {
                double arrr = sumSqXY / (stdDevX * stdDevY) / lookbackPeriods;
                r.RSquared = (arrr * arrr).NaN2Null();
            }
        }

        // add last Line (y = mx + b)
        if (length >= lookbackPeriods)
        {
            SlopeResult last = results.LastOrDefault();
            for (int p = length - lookbackPeriods; p < length; p++)
            {
                SlopeResult d = results[p];
                d.Line = (decimal?)((last.Slope * (p + 1)) + last.Intercept).NaN2Null();
            }
        }

        return results;
    }
}
