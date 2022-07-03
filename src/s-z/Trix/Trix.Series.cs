namespace Skender.Stock.Indicators;

// TRIPLE EMA OSCILLATOR - TRIX (SERIES)
public static partial class Indicator
{
    internal static List<TrixResult> CalcTrix(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods,
        int? signalPeriods)
    {
        // check parameter arguments
        ValidateTrix(lookbackPeriods);

        // initialize
        int length = tpList.Count;
        List<TrixResult> results = new(length);

        double k = 2d / (lookbackPeriods + 1);
        double? lastEma1 = 0;
        double? lastEma2;
        double? lastEma3;
        int initPeriods = Math.Min(lookbackPeriods, length);

        for (int i = 0; i < initPeriods; i++)
        {
            lastEma1 += tpList[i].Item2;
        }

        lastEma1 /= initPeriods;
        lastEma2 = lastEma3 = lastEma1;

        // compose final results
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];

            TrixResult r = new(date);
            results.Add(r);

            if (i >= lookbackPeriods)
            {
                double? ema1 = lastEma1 + (k * (value - lastEma1));
                double? ema2 = lastEma2 + (k * (ema1 - lastEma2));
                double? ema3 = lastEma3 + (k * (ema2 - lastEma3));

                r.Ema3 = ema3.NaN2Null();
                r.Trix = (100d * (ema3 - lastEma3) / lastEma3).NaN2Null();

                lastEma1 = ema1;
                lastEma2 = ema2;
                lastEma3 = ema3;
            }

            // optional SMA signal
            CalcTrixSignal(signalPeriods, i, lookbackPeriods, results);
        }

        return results;
    }

    // internals
    private static void CalcTrixSignal(
        int? signalPeriods, int i, int lookbackPeriods, List<TrixResult> results)
    {
        if (signalPeriods != null && i >= (lookbackPeriods + signalPeriods - 1))
        {
            double? sumSma = 0;
            for (int p = i + 1 - (int)signalPeriods; p <= i; p++)
            {
                sumSma += results[p].Trix;
            }

            results[i].Signal = sumSma / signalPeriods;
        }
    }

    // parameter validation
    private static void ValidateTrix(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for TRIX.");
        }
    }
}
