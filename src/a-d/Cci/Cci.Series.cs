namespace Skender.Stock.Indicators;

// COMMODITY CHANNEL INDEX (SERIES)
public static partial class Indicator
{
    internal static List<CciResult> CalcCci(
        this List<QuoteD> qdList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateCci(lookbackPeriods);

        // initialize
        int length = qdList.Count;
        List<CciResult> results = new(length);
        double[] tp = new double[length];

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD q = qdList[i];
            tp[i] = (q.High + q.Low + q.Close) / 3d;

            CciResult result = new(q.Date);
            results.Add(result);

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

                result.Cci = (avgDv == 0) ? double.NaN
                    : (tp[i] - avgTp) / (0.015 * avgDv);
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateCci(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Commodity Channel Index.");
        }
    }
}
