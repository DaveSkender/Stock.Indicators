namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (SERIES)

public static partial class Ema
{
    internal static List<EmaResult> CalcEma<T>(
        this List<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<EmaResult> results = new(length);

        double lastEma = double.NaN;
        double k = 2d / (lookbackPeriods + 1);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            // skip incalculable periods
            if (i < lookbackPeriods - 1)
            {
                results.Add(new(Timestamp: s.Timestamp));
                continue;
            }

            double ema = results[i - 1].Ema is not null

                // calculate EMA (normally)
                ? Ema.Increment(k, lastEma, s.Value)

                // when no prior EMA, reset as SMA
                : Sma.Increment(source, lookbackPeriods, i);

            EmaResult r = new(
                Timestamp: s.Timestamp,
                Ema: ema.NaN2Null());

            results.Add(r);

            lastEma = ema;
        }

        return results;
    }
}
