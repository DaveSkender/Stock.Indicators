namespace Skender.Stock.Indicators;

// ARNAUD LEGOUX MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    internal static List<AlmaResult> CalcAlma(
        this List<(DateTime TickDate, double _)> tpList,
        int lookbackPeriods,
        double offset,
        double sigma)
    {
        // check parameter arguments
        Alma.Validate(lookbackPeriods, offset, sigma);

        // initialize
        List<AlmaResult> results = new(tpList.Count);

        // determine price weights
        double m = offset * (lookbackPeriods - 1);
        double s = lookbackPeriods / sigma;

        double[] weight = new double[lookbackPeriods];
        double norm = 0;

        for (int i = 0; i < lookbackPeriods; i++)
        {
            double wt = Math.Exp(-((i - m) * (i - m)) / (2 * s * s));
            weight[i] = wt;
            norm += wt;
        }

        // roll through quotes
        for (int i = 0; i < tpList.Count; i++)
        {
            AlmaResult r = new() { TickDate = tpList[i].TickDate };
            results.Add(r);

            if (i + 1 >= lookbackPeriods)
            {
                double weightedSum = 0;
                int n = 0;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    (DateTime _, double pValue) = tpList[p];
                    weightedSum += weight[n] * pValue;
                    n++;
                }

                r.Alma = (weightedSum / norm).NaN2Null();
            }
        }

        return results;
    }
}
