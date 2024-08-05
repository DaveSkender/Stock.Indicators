namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (SERIES)

public static partial class Sma
{
    internal static List<SmaResult> CalcSma<T>(
        this List<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Sma.Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<SmaResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            double sma;

            if (i >= lookbackPeriods - 1)
            {
                double sumSma = 0;
                for (int p = i - lookbackPeriods + 1; p <= i; p++)
                {
                    T ps = source[p];
                    sumSma += ps.Value;
                }

                sma = sumSma / lookbackPeriods;
            }
            else
            {
                sma = double.NaN;
            }

            SmaResult result = new(
                Timestamp: s.Timestamp,
                Sma: sma.NaN2Null());

            results.Add(result);
        }

        return results;
    }
}
