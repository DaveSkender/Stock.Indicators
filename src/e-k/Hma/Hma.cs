namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // HULL MOVING AVERAGE
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<HmaResult> GetHma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // sort quotes
        List<TQuote> quotesList = quotes.SortToList();

        // check parameter arguments
        ValidateHma(lookbackPeriods);

        // initialize
        List<Quote> synthHistory = new();

        List<WmaResult> wmaN1 = GetWma(quotes, lookbackPeriods).ToList();
        List<WmaResult> wmaN2 = GetWma(quotes, lookbackPeriods / 2).ToList();

        // roll through quotes, to get interim synthetic quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            TQuote q = quotesList[i];

            Quote sh = new()
            {
                Date = q.Date
            };

            WmaResult w1 = wmaN1[i];
            WmaResult w2 = wmaN2[i];

            if (i >= lookbackPeriods - 1)
            {
                sh.Close = (w2.Wma * 2m) - w1.Wma;
                synthHistory.Add(sh);
            }
        }

        // add back truncated null results
        int sqN = (int)Math.Sqrt(lookbackPeriods);
        int shiftQty = lookbackPeriods - 1;

        List<HmaResult> results = quotesList
            .Take(shiftQty)
            .Select(x => new HmaResult
            {
                Date = x.Date
            })
            .ToList();

        // calculate final HMA = WMA with period SQRT(n)
        List<HmaResult> hmaResults = synthHistory.GetWma(sqN)
            .Select(x => new HmaResult
            {
                Date = x.Date,
                Hma = x.Wma
            })
            .ToList();

        // add WMA to results
        results.AddRange(hmaResults);
        results = results.OrderBy(x => x.Date).ToList();

        return results;
    }

    // parameter validation
    private static void ValidateHma(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for HMA.");
        }
    }
}
