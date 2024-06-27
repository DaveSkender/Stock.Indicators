namespace Skender.Stock.Indicators;

// TRIPLE EMA OSCILLATOR - TRIX (SERIES)

public static partial class Indicator
{
    internal static List<TrixResult> CalcTrix<T>(
        this List<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Trix.Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<TrixResult> results = new(length);

        double k = 2d / (lookbackPeriods + 1);
        double lastEma1 = double.NaN;
        double lastEma2 = double.NaN;
        double lastEma3 = double.NaN;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            var s = source[i];

            TrixResult r = new() { Timestamp = s.Timestamp };
            results.Add(r);

            // skip incalculable periods
            if (i < lookbackPeriods - 1)
            {
                continue;
            }

            double ema1;
            double ema2;
            double ema3;

            // when no prior EMA, reset as SMA
            if (double.IsNaN(lastEma3))
            {
                double sum = 0;
                for (int p = i - lookbackPeriods + 1; p <= i; p++)
                {
                    var ps = source[p];
                    sum += ps.Value;
                }

                ema1 = ema2 = ema3 = sum / lookbackPeriods;
            }

            // normal TRIX
            else
            {
                ema1 = lastEma1 + (k * (s.Value - lastEma1));
                ema2 = lastEma2 + (k * (ema1 - lastEma2));
                ema3 = lastEma3 + (k * (ema2 - lastEma3));

                r.Ema3 = ema3.NaN2Null();
                r.Trix = (100 * (ema3 - lastEma3) / lastEma3).NaN2Null();
            }

            lastEma1 = ema1;
            lastEma2 = ema2;
            lastEma3 = ema3;
        }

        return results;
    }
}
