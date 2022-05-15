namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // DETRENDED PRICE OSCILLATOR (DPO)
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<DpoResult> GetDpo<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // sort quotes
        List<TQuote> quotesList = quotes.SortToList();

        // check parameter arguments
        ValidateDpo(lookbackPeriods);

        // initialize
        int length = quotesList.Count;
        int offset = (lookbackPeriods / 2) + 1;
        List<SmaResult> sma = quotes.GetSma(lookbackPeriods).ToList();
        List<DpoResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            TQuote q = quotesList[i];

            DpoResult r = new()
            {
                Date = q.Date
            };
            results.Add(r);

            if (i >= lookbackPeriods - offset - 1 && i < length - offset)
            {
                SmaResult s = sma[i + offset];
                r.Sma = s.Sma;
                r.Dpo = s.Sma is null ? null : q.Close - s.Sma;
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateDpo(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for DPO.");
        }
    }
}
