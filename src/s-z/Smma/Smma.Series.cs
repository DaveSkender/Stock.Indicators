namespace Skender.Stock.Indicators;

// SMOOTHED MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    // calculate series
    private static List<SmmaResult> CalcSmma<T>(
        this List<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Smma.Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<SmmaResult> results = new(length);
        double prevSmma = double.NaN;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            // skip incalculable periods
            if (i < lookbackPeriods - 1)
            {
                results.Add(new() { Timestamp = s.Timestamp });
                continue;
            }

            double smma;

            // when no prior SMMA, reset as SMA
            if (double.IsNaN(prevSmma))
            {
                double sum = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    T ps = source[p];
                    sum += ps.Value;
                }

                smma = sum / lookbackPeriods;
            }

            // normal SMMA
            else
            {
                smma = (prevSmma * (lookbackPeriods - 1) + s.Value) / lookbackPeriods;
            }

            results.Add(new(
                Timestamp: s.Timestamp,
                Smma: smma.NaN2Null()));

            prevSmma = smma;
        }

        return results;
    }
}
