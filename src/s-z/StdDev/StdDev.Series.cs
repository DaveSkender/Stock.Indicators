namespace Skender.Stock.Indicators;

// STANDARD DEVIATION (SERIES)

public static partial class Indicator
{
    internal static List<StdDevResult> CalcStdDev<T>(
        this List<T> source,
        int lookbackPeriods)
        where T : IReusableResult
    {
        // check parameter arguments
        StdDev.Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<StdDevResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            var s = source[i];

            StdDevResult r = new() { Timestamp = s.Timestamp };
            results.Add(r);

            if (i + 1 >= lookbackPeriods)
            {
                double[] values = new double[lookbackPeriods];
                double sum = 0;
                int n = 0;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    var ps = source[p];
                    values[n] = ps.Value;
                    sum += ps.Value;
                    n++;
                }

                double avg = sum / lookbackPeriods;

                r.StdDev = values.StdDev().NaN2Null();
                r.Mean = avg.NaN2Null();

                r.ZScore = (r.StdDev == 0) ? null
                    : (s.Value - avg) / r.StdDev;
            }
        }

        return results;
    }
}
