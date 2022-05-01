namespace Skender.Stock.Indicators;
#nullable disable

public static partial class Indicator
{
    // SLOPE AND LINEAR REGRESSION
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<SlopeResult> GetSlope<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicData> bdList = quotes.ToBasicClass(CandlePart.Close);

        // check parameter arguments
        ValidateSlope(lookbackPeriods);

        // initialize
        int length = bdList.Count;
        List<SlopeResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            BasicData q = bdList[i];

            SlopeResult r = new()
            {
                Date = q.Date
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
                BasicData d = bdList[p];

                sumX += p + 1d;
                sumY += d.Value;
            }

            double avgX = sumX / lookbackPeriods;
            double avgY = sumY / lookbackPeriods;

            // least squares method
            double sumSqX = 0;
            double sumSqY = 0;
            double sumSqXY = 0;

            for (int p = i + 1 - lookbackPeriods; p <= i; p++)
            {
                BasicData d = bdList[p];

                double devX = p + 1d - avgX;
                double devY = d.Value - avgY;

                sumSqX += devX * devX;
                sumSqY += devY * devY;
                sumSqXY += devX * devY;
            }

            r.Slope = sumSqXY / sumSqX;
            r.Intercept = avgY - (r.Slope * avgX);

            // calculate Standard Deviation and R-Squared
            double stdDevX = Math.Sqrt((double)sumSqX / lookbackPeriods);
            double stdDevY = Math.Sqrt((double)sumSqY / lookbackPeriods);
            r.StdDev = stdDevY;

            if (stdDevX * stdDevY != 0)
            {
                double arrr = (double)sumSqXY / (stdDevX * stdDevY) / lookbackPeriods;
                r.RSquared = arrr * arrr;
            }
        }

        // add last Line (y = mx + b)
        if (length >= lookbackPeriods)
        {
            SlopeResult last = results.LastOrDefault();
            for (int p = length - lookbackPeriods; p < length; p++)
            {
                SlopeResult d = results[p];
                d.Line = (decimal?)((last.Slope * (p + 1)) + last.Intercept);
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
