namespace Skender.Stock.Indicators;

// TRIPLE EXPONENTIAL MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    // calculate series
    internal static List<TemaResult> CalcTema(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        Tema.Validate(lookbackPeriods);

        // initialize
        int length = tpList.Count;
        List<TemaResult> results = new(length);

        double k = 2d / (lookbackPeriods + 1);
        double lastEma1 = double.NaN;
        double lastEma2 = double.NaN;
        double lastEma3 = double.NaN;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];

            TemaResult r = new(date);
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
                    (DateTime _, double pValue) = tpList[p];
                    sum += pValue;
                }

                ema1 = ema2 = ema3 = sum / lookbackPeriods;
            }

            // normal TEMA
            else
            {
                ema1 = lastEma1 + (k * (value - lastEma1));
                ema2 = lastEma2 + (k * (ema1 - lastEma2));
                ema3 = lastEma3 + (k * (ema2 - lastEma3));
            }

            r.Tema = ((3 * ema1) - (3 * ema2) + ema3).NaN2Null();

            lastEma1 = ema1;
            lastEma2 = ema2;
            lastEma3 = ema3;
        }

        return results;
    }
}
