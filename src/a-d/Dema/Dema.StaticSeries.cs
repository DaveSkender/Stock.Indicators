namespace Skender.Stock.Indicators;

// DOUBLE EXPONENTIAL MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    private static List<DemaResult> CalcDema<T>(
        this List<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Dema.Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<DemaResult> results = new(length);

        double k = 2d / (lookbackPeriods + 1);
        double lastEma1 = double.NaN;
        double lastEma2 = double.NaN;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            // skip incalculable periods
            if (i < lookbackPeriods - 1)
            {
                results.Add(new(s.Timestamp));
                continue;
            }

            double ema1;
            double ema2;

            // when no prior EMA, reset as SMA
            if (double.IsNaN(lastEma2))
            {
                double sum = 0;
                for (int p = i - lookbackPeriods + 1; p <= i; p++)
                {
                    T ps = source[p];
                    sum += ps.Value;
                }

                ema1 = ema2 = sum / lookbackPeriods;
            }

            // normal DEMA
            else
            {
                ema1 = lastEma1 + (k * (s.Value - lastEma1));
                ema2 = lastEma2 + (k * (ema1 - lastEma2));
            }

            results.Add(new DemaResult(
                Timestamp: s.Timestamp,
                Dema: ((2d * ema1) - ema2).NaN2Null()));

            lastEma1 = ema1;
            lastEma2 = ema2;
        }

        return results;
    }
}
