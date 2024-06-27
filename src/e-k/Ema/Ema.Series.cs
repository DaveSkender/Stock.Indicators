namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    internal static List<EmaResult> CalcEma<T>(
        this List<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        EmaUtilities.Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<EmaResult> results = new(length);

        double lastEma = double.NaN;
        double k = 2d / (lookbackPeriods + 1);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            var s = source[i];

            // skip incalculable periods
            if (i < lookbackPeriods - 1)
            {
                results.Add(new EmaResult(Timestamp: s.Timestamp));
                continue;
            }

            double ema;

            // when no prior EMA, reset as SMA
            if (double.IsNaN(lastEma))
            {
                double sum = 0;
                for (int p = i - lookbackPeriods + 1; p <= i; p++)
                {
                    var ps = source[p];
                    sum += ps.Value;
                }

                ema = sum / lookbackPeriods;
            }

            // normal EMA
            else
            {
                ema = EmaUtilities.Increment(k, lastEma, s.Value);
            }

            EmaResult r = new(
                Timestamp: s.Timestamp,
                Ema: ema.NaN2Null());

            results.Add(r);

            lastEma = ema;
        }

        return results;
    }
}
