namespace Skender.Stock.Indicators;
#nullable disable

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
        List<BaseQuote> bdList = quotes.ToBaseQuote(CandlePart.Close);

        // check parameter arguments
        ValidateTrix(lookbackPeriods);

        // initialize
        List<TrixResult> results = new(bdList.Count);
        decimal? lastEma = null;

        List<EmaResult> emaN1 = CalcEma(bdList, lookbackPeriods);

        List<BaseQuote> bd2 = emaN1
            .Where(x => x.Ema != null)
            .Select(x => new BaseQuote { Date = x.Date, Value = (double)x.Ema })
            .ToList();

        List<EmaResult> emaN2 = CalcEma(bd2, lookbackPeriods);

        List<BaseQuote> bd3 = emaN2
            .Where(x => x.Ema != null)
            .Select(x => new BaseQuote { Date = x.Date, Value = (double)x.Ema })
            .ToList();

        List<EmaResult> emaN3 = CalcEma(bd3, lookbackPeriods);

        // compose final results
        for (int i = 0; i < emaN1.Count; i++)
        {
            EmaResult e1 = emaN1[i];
            int index = i + 1;

            TrixResult result = new()
            {
                Date = e1.Date
            };

            results.Add(result);

            if (index >= (3 * lookbackPeriods) - 2)
            {
                EmaResult e3 = emaN3[index - (2 * lookbackPeriods) + 1];

                result.Ema3 = e3.Ema;

                if (lastEma is not null and not 0)
                {
                    result.Trix = 100 * (e3.Ema - lastEma) / lastEma;
                }

                lastEma = e3.Ema;

                // optional SMA signal
                GetTrixSignal(signalPeriods, index, lookbackPeriods, results);
            }
        }

        return results;
    }

    // internals
    private static void GetTrixSignal(
        int? signalPeriods, int index, int lookbackPeriods, List<TrixResult> results)
    {
        if (signalPeriods != null && index >= (3 * lookbackPeriods) - 2 + signalPeriods)
        {
            decimal sumSma = 0m;
            for (int p = index - (int)signalPeriods; p < index; p++)
            {
                sumSma += (decimal)results[p].Trix;
            }

            results[index - 1].Signal = sumSma / signalPeriods;
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
