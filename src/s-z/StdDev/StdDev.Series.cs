namespace Skender.Stock.Indicators;

// STANDARD DEVIATION (SERIES)

public static partial class Indicator
{
    private static List<StdDevResult> CalcStdDev<T>(
        this List<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        StdDev.Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<StdDevResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            double mean;
            double stdDev;
            double zScore;

            if (i >= lookbackPeriods - 1)
            {
                double[] values = new double[lookbackPeriods];
                double sum = 0;
                int n = 0;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    T ps = source[p];
                    values[n] = ps.Value;
                    sum += ps.Value;
                    n++;
                }

                mean = sum / lookbackPeriods;

                stdDev = values.StdDev();

                zScore = stdDev == 0 ? double.NaN
                    : (s.Value - mean) / stdDev;
            }
            else
            {
                mean = double.NaN;
                stdDev = double.NaN;
                zScore = double.NaN;
            }

            StdDevResult r = new(
                Timestamp: s.Timestamp,
                StdDev: stdDev.NaN2Null(),
                Mean: mean.NaN2Null(),
                ZScore: zScore.NaN2Null());

            results.Add(r);
        }

        return results;
    }
}
