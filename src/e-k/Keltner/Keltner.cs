namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // KELTNER CHANNELS
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<KeltnerResult> GetKeltner<TQuote>(
        this IEnumerable<TQuote> quotes,
        int emaPeriods = 20,
        decimal multiplier = 2,
        int atrPeriods = 10)
        where TQuote : IQuote
    {
        // sort quotes
        List<TQuote> quotesList = quotes.SortToList();

        // check parameter arguments
        ValidateKeltner(quotes, emaPeriods, multiplier, atrPeriods);

        // initialize
        List<KeltnerResult> results = new(quotesList.Count);
        List<EmaResult> emaResults = GetEma(quotes, emaPeriods).ToList();
        List<AtrResult> atrResults = GetAtr(quotes, atrPeriods).ToList();
        int lookbackPeriods = Math.Max(emaPeriods, atrPeriods);

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            TQuote q = quotesList[i];
            int index = i + 1;

            KeltnerResult result = new()
            {
                Date = q.Date
            };

            if (index >= lookbackPeriods)
            {
                EmaResult ema = emaResults[i];
                AtrResult atr = atrResults[i];

                result.UpperBand = ema.Ema + (multiplier * atr.Atr);
                result.LowerBand = ema.Ema - (multiplier * atr.Atr);
                result.Centerline = ema.Ema;
                result.Width = (result.Centerline == 0) ? null
                    : (result.UpperBand - result.LowerBand) / result.Centerline;
            }

            results.Add(result);
        }

        return results;
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<KeltnerResult> RemoveWarmupPeriods(
        this IEnumerable<KeltnerResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Width != null) + 1;

        return results.Remove(Math.Max(2 * n, n + 100));
    }

    // parameter validation
    private static void ValidateKeltner<TQuote>(
        IEnumerable<TQuote> quotes,
        int emaPeriods,
        decimal multiplier,
        int atrPeriods)
        where TQuote : IQuote
    {
        // check parameter arguments
        if (emaPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(emaPeriods), emaPeriods,
                "EMA periods must be greater than 1 for Keltner Channel.");
        }

        if (atrPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(atrPeriods), atrPeriods,
                "ATR periods must be greater than 1 for Keltner Channel.");
        }

        if (multiplier <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(multiplier), multiplier,
                "Multiplier must be greater than 0 for Keltner Channel.");
        }

        // check quotes
        int lookbackPeriods = Math.Max(emaPeriods, atrPeriods);
        int qtyHistory = quotes.Count();
        int minHistory = Math.Max(2 * lookbackPeriods, lookbackPeriods + 100);
        if (qtyHistory < minHistory)
        {
            string message = "Insufficient quotes provided for Keltner Channel.  " +
                string.Format(
                    EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.  "
                + "Since this uses a smoothing technique, for {2} lookback periods "
                + "we recommend you use at least {3} data points prior to the intended "
                + "usage date for better precision.",
                    qtyHistory, minHistory, lookbackPeriods, lookbackPeriods + 250);

            throw new BadQuotesException(nameof(quotes), message);
        }
    }
}
