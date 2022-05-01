namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // TRIPLE EMA OSCILLATOR (TRIX)
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<TrixResult> GetTrix<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        int? signalPeriods = null)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicData> bdList = quotes.ToBasicClass(CandlePart.Close);

        // check parameter arguments
        ValidateTrix(lookbackPeriods);

        // initialize
        int length = bdList.Count;
        List<TrixResult> results = new(length);

        double k = 2d / (lookbackPeriods + 1);
        double? lastEma1 = 0;
        double? lastEma2;
        double? lastEma3;
        int initPeriods = Math.Min(lookbackPeriods, length);

        for (int i = 0; i < initPeriods; i++)
        {
            lastEma1 += bdList[i].Value;
        }

        lastEma1 /= lookbackPeriods;
        lastEma2 = lastEma3 = lastEma1;

        // compose final results
        for (int i = 0; i < length; i++)
        {

            BasicData q = bdList[i];

            TrixResult result = new()
            {
                Date = q.Date
            };

            if (i >= lookbackPeriods)
            {
                double? ema1 = lastEma1 + (k * (q.Value - lastEma1));
                double? ema2 = lastEma2 + (k * (ema1 - lastEma2));
                double? ema3 = lastEma3 + (k * (ema2 - lastEma3));

                result.Ema3 = ema3;
                result.Trix = 100 * (ema3 - lastEma3) / lastEma3;

                lastEma1 = ema1;
                lastEma2 = ema2;
                lastEma3 = ema3;
            }

            results.Add(result);

            // optional SMA signal
            GetTrixSignal(signalPeriods, i, lookbackPeriods, results);
        }

        return results;
    }

    // internals
    private static void GetTrixSignal(
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
