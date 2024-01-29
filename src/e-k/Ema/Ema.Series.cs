namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    internal static List<EmaResult> CalcEma(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        Ema.Validate(lookbackPeriods);

        // initialize
        int length = tpList.Count;
        List<EmaResult> results = new(length);

        double lastEma = double.NaN;
        double k = 2d / (lookbackPeriods + 1);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];

            EmaResult r = new() { TickDate = date };
            results.Add(r);

            // skip incalculable periods
            if (i < lookbackPeriods - 1)
            {
                continue;
            }

            double ema;

            // when no prior EMA, reset as SMA
            if (double.IsNaN(lastEma))
            {
                double sum = 0;
                for (int p = i - lookbackPeriods + 1; p <= i; p++)
                {
                    (DateTime _, double pValue) = tpList[p];
                    sum += pValue;
                }

                ema = sum / lookbackPeriods;
            }

            // normal EMA
            else
            {
                ema = Ema.Increment(k, lastEma, value);
            }

            r.Ema = ema.NaN2Null();
            lastEma = ema;
        }

        return results;
    }
}
