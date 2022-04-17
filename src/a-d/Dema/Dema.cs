namespace Skender.Stock.Indicators;
#nullable disable

public static partial class Indicator
{
    // DOUBLE EXPONENTIAL MOVING AVERAGE (DEMA)
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<DemaResult> GetDema<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // convert quotes
        List<SimplePrice> bdList = quotes.ToPrice(CandlePart.Close);

        // check parameter arguments
        ValidateDema(lookbackPeriods);

        // initialize
        List<DemaResult> results = new(bdList.Count);
        List<EmaResult> emaN = CalcEma(bdList, lookbackPeriods);

        List<SimplePrice> bd2 = emaN
            .Where(x => x.Ema != null)
            .Select(x => new SimplePrice { Date = x.Date, Value = (double)x.Ema })
            .ToList();

        List<EmaResult> emaN2 = CalcEma(bd2, lookbackPeriods);

        // compose final results
        for (int i = 0; i < emaN.Count; i++)
        {
            EmaResult e1 = emaN[i];
            int index = i + 1;

            DemaResult result = new()
            {
                Date = e1.Date
            };

            if (index >= (2 * lookbackPeriods) - 1)
            {
                EmaResult e2 = emaN2[index - lookbackPeriods];
                result.Dema = (2 * e1.Ema) - e2.Ema;
            }

            results.Add(result);
        }

        return results;
    }

    // parameter validation
    private static void ValidateDema(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for DEMA.");
        }
    }
}
