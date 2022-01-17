namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // DONCHIAN CHANNEL
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<DonchianResult> GetDonchian<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 20)
        where TQuote : IQuote
    {
        // sort quotes
        List<TQuote> quotesList = quotes.SortToList();

        // check parameter arguments
        ValidateDonchian(quotes, lookbackPeriods);

        // initialize
        List<DonchianResult> results = new(quotesList.Count);

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            TQuote q = quotesList[i];

            DonchianResult result = new()
            {
                Date = q.Date
            };

            if (i >= lookbackPeriods)
            {
                decimal highHigh = 0;
                decimal lowLow = decimal.MaxValue;

                // high/low over prior periods
                for (int p = i - lookbackPeriods; p < i; p++)
                {
                    TQuote d = quotesList[p];

                    if (d.High > highHigh)
                    {
                        highHigh = d.High;
                    }

                    if (d.Low < lowLow)
                    {
                        lowLow = d.Low;
                    }
                }

                result.UpperBand = highHigh;
                result.LowerBand = lowLow;
                result.Centerline = (result.UpperBand + result.LowerBand) / 2;
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
    public static IEnumerable<DonchianResult> RemoveWarmupPeriods(
        this IEnumerable<DonchianResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.Width != null);

        return results.Remove(removePeriods);
    }

    // parameter validation
    private static void ValidateDonchian<TQuote>(
        IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Donchian Channel.");
        }

        // check quotes
        int qtyHistory = quotes.Count();
        int minHistory = lookbackPeriods + 1;
        if (config.UseBadQuotesException && qtyHistory < minHistory)
        {
            string message = "Insufficient quotes provided for Donchian Channel.  " +
                string.Format(
                    EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

            throw new BadQuotesException(nameof(quotes), message);
        }
    }
}
