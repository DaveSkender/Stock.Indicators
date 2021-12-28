namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // SUPERTREND
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<SuperTrendResult> GetSuperTrend<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 10,
        double multiplier = 3)
        where TQuote : IQuote
    {
        // convert quotes
        List<QuoteD> quotesList = quotes.ConvertToList();

        // check parameter arguments
        ValidateSuperTrend(quotes, lookbackPeriods, multiplier);

        // initialize
        List<SuperTrendResult> results = new(quotesList.Count);
        List<AtrResult> atrResults = GetAtr(quotes, lookbackPeriods).ToList();

        bool isBullish = true;
        double? upperBand = null;
        double? lowerBand = null;

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            QuoteD q = quotesList[i];

            SuperTrendResult r = new()
            {
                Date = q.Date
            };

            if (i >= lookbackPeriods - 1)
            {
                double mid = (q.High + q.Low) / 2;
                double atr = (double)atrResults[i].Atr;
                double prevClose = quotesList[i - 1].Close;

                // potential bands
                double upperEval = mid + (multiplier * atr);
                double lowerEval = mid - (multiplier * atr);

                // initial values
                if (i == lookbackPeriods - 1)
                {
                    isBullish = q.Close >= mid;

                    upperBand = upperEval;
                    lowerBand = lowerEval;
                }

                // new upper band
                if (upperEval < upperBand || prevClose > upperBand)
                {
                    upperBand = upperEval;
                }

                // new lower band
                if (lowerEval > lowerBand || prevClose < lowerBand)
                {
                    lowerBand = lowerEval;
                }

                // supertrend
                if (q.Close <= (isBullish ? lowerBand : upperBand))
                {
                    r.SuperTrend = (decimal?)upperBand;
                    r.UpperBand = (decimal?)upperBand;
                    isBullish = false;
                }
                else
                {
                    r.SuperTrend = (decimal?)lowerBand;
                    r.LowerBand = (decimal?)lowerBand;
                    isBullish = true;
                }
            }

            results.Add(r);
        }

        return results;
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<SuperTrendResult> RemoveWarmupPeriods(
        this IEnumerable<SuperTrendResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.SuperTrend != null);

        return results.Remove(removePeriods);
    }

    // parameter validation
    private static void ValidateSuperTrend<TQuote>(
        IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        double multiplier)
        where TQuote : IQuote
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for SuperTrend.");
        }

        if (multiplier <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(multiplier), multiplier,
                "Multiplier must be greater than 0 for SuperTrend.");
        }

        // check quotes
        int qtyHistory = quotes.Count();
        int minHistory = lookbackPeriods + 100;
        if (qtyHistory < minHistory)
        {
            string message = "Insufficient quotes provided for SuperTrend.  " +
                string.Format(
                    EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.  "
                + "Since this uses a smoothing technique, "
                + "we recommend you use at least N+250 data points prior to the intended "
                + "usage date for better precision.", qtyHistory, minHistory);

            throw new BadQuotesException(nameof(quotes), message);
        }
    }
}
