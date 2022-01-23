namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // ElderRay
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<ElderRayResult> GetElderRay<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 13)
        where TQuote : IQuote
    {
        // sort quotes
        List<TQuote> quotesList = quotes.SortToList();

        // check parameter arguments
        ValidateElderRay(quotes, lookbackPeriods);

        // initialize with EMA
        List<ElderRayResult> results = GetEma(quotes, lookbackPeriods)
            .Select(x => new ElderRayResult
            {
                Date = x.Date,
                Ema = x.Ema
            })
            .ToList();

        // roll through quotes
        for (int i = lookbackPeriods - 1; i < quotesList.Count; i++)
        {
            TQuote q = quotesList[i];
            ElderRayResult r = results[i];

            r.BullPower = q.High - r.Ema;
            r.BearPower = q.Low - r.Ema;
        }

        return results;
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<ElderRayResult> RemoveWarmupPeriods(
        this IEnumerable<ElderRayResult> results)
    {
        int n = results
          .ToList()
          .FindIndex(x => x.BullPower != null) + 1;

        return results.Remove(n + 100);
    }

    // parameter validation
    private static void ValidateElderRay<TQuote>(
        IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Elder-ray Index.");
        }

        // check quotes
        int qtyHistory = quotes.Count();
        int minHistory = Math.Max(2 * lookbackPeriods, lookbackPeriods + 100);
        if (config.UseBadQuotesException && qtyHistory < minHistory)
        {
            string message = "Insufficient quotes provided for Elder-ray Index.  " +
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
