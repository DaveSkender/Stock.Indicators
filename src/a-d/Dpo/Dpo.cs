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
                r.Dpo = q.Close - s.Sma;
            }
        }

        return results;
    }

    // convert to quotes
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Convert"]/*' />
    ///
    public static IEnumerable<Quote> ConvertToQuotes(
        this IEnumerable<DpoResult> results)
    {
        return results
          .Where(x => x.Dpo != null)
          .Select(x => new Quote
          {
              Date = x.Date,
              Open = (decimal)x.Dpo,
              High = (decimal)x.Dpo,
              Low = (decimal)x.Dpo,
              Close = (decimal)x.Dpo,
              Volume = (decimal)x.Dpo
          })
          .ToList();
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
