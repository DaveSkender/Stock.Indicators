namespace Skender.Stock.Indicators;

// COMMODITY CHANNEL INDEX (SERIES)

public static partial class Indicator
{
    private static List<CciResult> CalcCci(
        this List<QuoteD> source,
        int lookbackPeriods)
    {
        // check parameter arguments
        Cci.Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<CciResult> results = new(length);
        double[] tp = new double[length];

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = source[i];
            tp[i] = (q.High + q.Low + q.Close) / 3d;

            double? cci = null;

            if (i + 1 >= lookbackPeriods)
            {
                // average TP over lookback
                double avgTp = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    avgTp += tp[p];
                }

                avgTp /= lookbackPeriods;

                // average Deviation over lookback
                double avgDv = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    avgDv += Math.Abs(avgTp - tp[p]);
                }

                avgDv /= lookbackPeriods;

                cci = avgDv == 0 ? null
                    : ((tp[i] - avgTp) / (0.015 * avgDv)).NaN2Null();
            }

            results.Add(new(
                Timestamp: q.Timestamp,
                Cci: cci));
        }

        return results;
    }
}
