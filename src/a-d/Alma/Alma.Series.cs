namespace Skender.Stock.Indicators;

// ARNAUD LEGOUX MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    internal static List<AlmaResult> CalcAlma<T>(
        this List<T> source,
        int lookbackPeriods,
        double offset,
        double sigma)
        where T : IReusableResult
    {
        // check parameter arguments
        Alma.Validate(lookbackPeriods, offset, sigma);

        // initialize
        List<AlmaResult> results = new(source.Count);

        // determine price weight constants
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
        for (int i = 0; i < source.Count; i++)
        {
            double alma = double.NaN;

            if (i + 1 >= lookbackPeriods)
            {
                double weightedSum = 0;
                int n = 0;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    T ps = source[p];
                    weightedSum += weight[n] * ps.Value;
                    n++;
                }

                alma = weightedSum / norm;
            }

            results.Add(
            new(Timestamp: source[i].Timestamp,
                Alma: alma.NaN2Null()));
        }

        return results;
    }
}
