namespace Skender.Stock.Indicators;
#nullable disable

public static partial class Indicator
{
    // TRIPLE EXPONENTIAL MOVING AVERAGE
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<TemaResult> GetTripleEma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicD> bdList = quotes.ConvertToBasic(CandlePart.Close);

        // check parameter arguments
        ValidateTema(lookbackPeriods);

        // initialize
        List<TemaResult> results = new(bdList.Count);
        List<EmaResult> emaN1 = CalcEma(bdList, lookbackPeriods);

        List<BasicD> bd2 = emaN1
            .Where(x => x.Ema != null)
            .Select(x => new BasicD { Date = x.Date, Value = (double)x.Ema })
            .ToList();

        List<EmaResult> emaN2 = CalcEma(bd2, lookbackPeriods);

        List<BasicD> bd3 = emaN2
            .Where(x => x.Ema != null)
            .Select(x => new BasicD { Date = x.Date, Value = (double)x.Ema })
            .ToList();

        List<EmaResult> emaN3 = CalcEma(bd3, lookbackPeriods);

        // compose final results
        for (int i = 0; i < emaN1.Count; i++)
        {
            EmaResult e1 = emaN1[i];
            int index = i + 1;

            TemaResult result = new()
            {
                Date = e1.Date
            };

            if (index >= (3 * lookbackPeriods) - 2)
            {
                EmaResult e2 = emaN2[index - lookbackPeriods];
                EmaResult e3 = emaN3[index - (2 * lookbackPeriods) + 1];

                result.Tema = (3 * e1.Ema) - (3 * e2.Ema) + e3.Ema;
            }

            results.Add(result);
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
