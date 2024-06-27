namespace Skender.Stock.Indicators;

// WEIGHTED MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    internal static List<WmaResult> CalcWma<T>(
        this List<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Wma.Validate(lookbackPeriods);

        // initialize
        List<WmaResult> results = new(source.Count);
        double divisor = (double)lookbackPeriods * (lookbackPeriods + 1) / 2d;

        // roll through quotes
        for (int i = 0; i < source.Count; i++)
        {
            var s = source[i];

            WmaResult r = new() { Timestamp = s.Timestamp };
            results.Add(r);

            if (i + 1 >= lookbackPeriods)
            {
                double wma = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    var ps = source[p];
                    wma += ps.Value * (lookbackPeriods - (i + 1 - p - 1)) / divisor;
                }

                r.Wma = wma.NaN2Null();
            }
        }

        return results;
    }
}
