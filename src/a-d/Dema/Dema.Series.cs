namespace Skender.Stock.Indicators;

// DOUBLE EXPONENTIAL MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    internal static List<DemaResult> CalcDema(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        Dema.Validate(lookbackPeriods);

        // initialize
        int length = tpList.Count;
        List<DemaResult> results = new(length);

        double k = 2d / (lookbackPeriods + 1);
        double lastEma1 = double.NaN;
        double lastEma2 = double.NaN;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];

            DemaResult r = new() { Date = date };
            results.Add(r);

            // skip incalculable periods
            if (i < lookbackPeriods - 1)
            {
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
                    (DateTime _, double pValue) = tpList[p];
                    sum += pValue;
                }

                ema1 = ema2 = sum / lookbackPeriods;
            }

            // normal DEMA
            else
            {
                ema1 = lastEma1 + (k * (value - lastEma1));
                ema2 = lastEma2 + (k * (ema1 - lastEma2));
            }

            r.Dema = ((2d * ema1) - ema2).NaN2Null();

            lastEma1 = ema1;
            lastEma2 = ema2;
        }

        return results;
    }
}
