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
        SmaResult[] results = new SmaResult[length];
        double[] values = new double[length];

        for (int i = 0; i < length; i++)
        {
            values[i] = source[i].Value;
        }

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            double sma;

            if (i >= lookbackPeriods - 1)
            {
                double sum = 0;
                int end = i + 1;
                int start = end - lookbackPeriods;

                for (int p = start; p < end; p++)
                {
                    sum += source[p].Value;
                }

                sma = sum / lookbackPeriods;
            }
            else
            {
                sma = double.NaN;
            }

            results[i] = new SmaResult(
                Timestamp: s.Timestamp,
                Sma: sma.NaN2Null());
        }

        return new List<SmaResult>(results);
    }
}
