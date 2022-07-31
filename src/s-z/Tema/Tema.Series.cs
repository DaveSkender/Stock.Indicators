namespace Skender.Stock.Indicators;

// TRIPLE EXPONENTIAL MOVING AVERAGE - TEMA (SERIES)
public static partial class Indicator
{
    // calculate series
    internal static List<TemaResult> CalcTema(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateTema(lookbackPeriods);

        // initialize
        int length = tpList.Count;
        List<TemaResult> results = new(length);

        double k = 2d / (lookbackPeriods + 1);
        double? lastEma1 = 0;
        double? lastEma2;
        double? lastEma3;
        int initPeriods = Math.Min(lookbackPeriods, length);

        for (int i = 0; i < initPeriods; i++)
        {
            (DateTime _, double value) = tpList[i];
            lastEma1 += value;
        }

        lastEma1 /= lookbackPeriods;
        lastEma2 = lastEma3 = lastEma1;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];

            TemaResult r = new(date);
            results.Add(r);

            if (i > lookbackPeriods - 1)
            {
                double? ema1 = lastEma1 + (k * (value - lastEma1));
                double? ema2 = lastEma2 + (k * (ema1 - lastEma2));
                double? ema3 = lastEma3 + (k * (ema2 - lastEma3));

                r.Tema = ((3 * ema1) - (3 * ema2) + ema3).NaN2Null();

                lastEma1 = ema1;
                lastEma2 = ema2;
                lastEma3 = ema3;
            }
            else if (i == lookbackPeriods - 1)
            {
                r.Tema = ((3 * lastEma1) - (3 * lastEma2) + lastEma3).NaN2Null();
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateTema(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for TEMA.");
        }
    }
}
