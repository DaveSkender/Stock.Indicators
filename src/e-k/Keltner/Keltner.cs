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
        List<TQuote> quotesList = quotes.ToSortedList();

        // check parameter arguments
        ValidateKeltner(emaPeriods, multiplier, atrPeriods);

        // initialize
        List<KeltnerResult> results = new(quotesList.Count);
        List<EmaResult> emaResults = GetEma(quotes, emaPeriods).ToList();
        List<AtrResult> atrResults = GetAtr(quotes, atrPeriods).ToList();
        int lookbackPeriods = Math.Max(emaPeriods, atrPeriods);

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            TQuote q = quotesList[i];

            KeltnerResult result = new()
            {
                Date = q.Date
            };

            if (i + 1 >= lookbackPeriods)
            {
                EmaResult ema = emaResults[i];
                AtrResult atr = atrResults[i];
                decimal? emaM = (decimal?)ema.Ema;

                result.UpperBand = emaM + (multiplier * atr.Atr);
                result.LowerBand = emaM - (multiplier * atr.Atr);
                result.Centerline = emaM;
                result.Width = (result.Centerline == 0) ? null
                    : (result.UpperBand - result.LowerBand) / result.Centerline;
            }

            results.Add(result);
        }

        return results;
    }

    // parameter validation
    private static void ValidateKeltner(
        int emaPeriods,
        decimal multiplier,
        int atrPeriods)
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
    }
}
