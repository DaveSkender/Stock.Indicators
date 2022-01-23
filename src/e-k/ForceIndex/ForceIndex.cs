namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // FORCE INDEX
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<ForceIndexResult> GetForceIndex<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // convert quotes
        List<QuoteD> quotesList = quotes.ConvertToList();

        // check parameter arguments
        ValidateForceIndex(quotes, lookbackPeriods);

        // initialize
        int size = quotesList.Count;
        List<ForceIndexResult> results = new(size);
        double? prevClose = null, prevFI = null, sumRawFI = 0;
        double k = 2d / (lookbackPeriods + 1);

        // roll through quotes
        for (int i = 0; i < size; i++)
        {
            QuoteD q = quotesList[i];
            int index = i + 1;

            ForceIndexResult r = new()
            {
                Date = q.Date
            };
            results.Add(r);

            // skip first period
            if (i == 0)
            {
                prevClose = q.Close;
                continue;
            }

            // raw Force Index
            double? rawFI = q.Volume * (q.Close - prevClose);
            prevClose = q.Close;

            // calculate EMA
            if (index > lookbackPeriods + 1)
            {
                r.ForceIndex = prevFI + (k * (rawFI - prevFI));
            }

            // initialization period
            else
            {
                sumRawFI += rawFI;

                // first EMA value
                if (index == lookbackPeriods + 1)
                {
                    r.ForceIndex = sumRawFI / lookbackPeriods;
                }
            }

            prevFI = r.ForceIndex;
        }

        return results;
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<ForceIndexResult> RemoveWarmupPeriods(
        this IEnumerable<ForceIndexResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.ForceIndex != null);

        return results.Remove(n + 100);
    }

    // parameter validation
    private static void ValidateForceIndex<TQuote>(
        IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Force Index.");
        }

        // check quotes
        int qtyHistory = quotes.Count();
        int minHistory = Math.Max(2 * lookbackPeriods, lookbackPeriods + 100);
        if (config.UseBadQuotesException && qtyHistory < minHistory)
        {
            string message = "Insufficient quotes provided for Force Index.  " +
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
