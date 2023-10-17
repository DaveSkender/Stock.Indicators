namespace Skender.Stock.Indicators;

// COMMODITY CHANNEL INDEX (SERIES)

public static partial class Indicator
{
    internal static List<CciResult> CalcCci(
        this List<QuoteD> qdList,
        int lookbackPeriods)
    {
        // check parameter arguments
        Cci.Validate(lookbackPeriods);

        // initialize
        int length = qdList.Count;
        List<CciResult> results = new(length);
        double[] tp = new double[length];

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD q = qdList[i];
            tp[i] = (q.High + q.Low + q.Close) / 3d;

            CciResult r = new(q.Date);
            results.Add(r);

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

                r.Cci = (avgDv == 0) ? null
                    : ((tp[i] - avgTp) / (0.015 * avgDv)).NaN2Null();
            }
        }

        return results;
    }
}
