namespace Skender.Stock.Indicators;

// SLOPE AND LINEAR REGRESSION (SERIES)
public static partial class Indicator
{
    // calculate series
    internal static IEnumerable<SlopeResult> CalcSlope(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateSlope(lookbackPeriods);

        // initialize
        int length = tpList.Count;
        List<SlopeResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];

            SlopeResult r = new()
            {
                Date = date
            };

            results.Add(r);

            // skip initialization period
            if (i + 1 < lookbackPeriods)
            {
                continue;
            }

            // get averages for period
            double sumX = 0;
            double sumY = 0;

            for (int p = i + 1 - lookbackPeriods; p <= i; p++)
            {
                (DateTime pDate, double pValue) = tpList[p];

                sumX += p + 1d;
                sumY += pValue;
            }

            double avgX = sumX / lookbackPeriods;
            double avgY = sumY / lookbackPeriods;

            // least squares method
            double sumSqX = 0;
            double sumSqY = 0;
            double sumSqXY = 0;

            for (int p = i + 1 - lookbackPeriods; p <= i; p++)
            {
                (DateTime pDate, double pValue) = tpList[p];

                double devX = p + 1d - avgX;
                double devY = pValue - avgY;

                sumSqX += devX * devX;
                sumSqY += devY * devY;
                sumSqXY += devX * devY;
            }

            r.Slope = sumSqXY / sumSqX;
            r.Intercept = avgY - (r.Slope * avgX);

            // calculate Standard Deviation and R-Squared
            double stdDevX = Math.Sqrt(sumSqX / lookbackPeriods);
            double stdDevY = Math.Sqrt(sumSqY / lookbackPeriods);
            r.StdDev = stdDevY;

            if (stdDevX * stdDevY != 0)
            {
                double arrr = sumSqXY / (stdDevX * stdDevY) / lookbackPeriods;
                r.RSquared = arrr * arrr;
            }
        }

        // add last Line (y = mx + b)
        if (length >= lookbackPeriods)
        {
            SlopeResult? last = results.LastOrDefault();
            for (int p = length - lookbackPeriods; p < length; p++)
            {
                SlopeResult d = results[p];
                d.Line = (decimal?)((last?.Slope * (p + 1)) + last?.Intercept);
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateSlope(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Slope/Linear Regression.");
        }
    }
}
