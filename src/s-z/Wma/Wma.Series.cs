namespace Skender.Stock.Indicators;

// WEIGHTED MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    private static List<WmaResult> CalcWma<T>(
        this List<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Wma.Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<WmaResult> results = new(length);

        double divisor = (double)lookbackPeriods * (lookbackPeriods + 1) / 2d;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            double wma;

            if (i >= lookbackPeriods - 1)
            {
                wma = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    T ps = source[p];
                    wma += ps.Value * (lookbackPeriods - (i + 1 - p - 1)) / divisor;
                }
            }
            else
            {
                wma = double.NaN;
            }

            results.Add(new(
                Timestamp: s.Timestamp,
                Wma: wma.NaN2Null()));
        }

        return results;
    }
}
