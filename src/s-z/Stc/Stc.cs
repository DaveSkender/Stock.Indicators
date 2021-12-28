namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // SCHAFF TREND CYCLE (STC)
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<StcResult> GetStc<TQuote>(
        this IEnumerable<TQuote> quotes,
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50)
        where TQuote : IQuote
    {

        // convert quotes
        List<BasicD> quotesList = quotes.ConvertToBasic(CandlePart.Close);

        // check parameter arguments
        ValidateStc(quotes, cyclePeriods, fastPeriods, slowPeriods);

        // get stochastic of macd
        IEnumerable<StochResult> stochMacd = quotes
            .GetMacd(fastPeriods, slowPeriods, 1)
            .Where(x => x.Macd != null)
            .Select(x => new Quote
            {
                Date = x.Date,
                High = (decimal)x.Macd,
                Low = (decimal)x.Macd,
                Close = (decimal)x.Macd
            })
            .GetStoch(cyclePeriods, 1, 3);

        // initialize results
        // to ensure same length as original quotes
        List<StcResult> results = new(quotesList.Count);

        for (int i = 0; i < slowPeriods - 1; i++)
        {
            BasicD q = quotesList[i];
            results.Add(new StcResult() { Date = q.Date });
        }

        // add stoch results
        // TODO: see if List Add works faster
        results.AddRange(
           stochMacd
           .Select(x => new StcResult
           {
               Date = x.Date,
               Stc = x.Oscillator
           }));

        return results;
    }


    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<StcResult> RemoveWarmupPeriods(
        this IEnumerable<StcResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Stc != null);

        return results.Remove(n + 250);
    }


    // parameter validation
    private static void ValidateStc<TQuote>(
        IEnumerable<TQuote> quotes,
        int cyclePeriods,
        int fastPeriods,
        int slowPeriods)
        where TQuote : IQuote
    {

        // check parameter arguments
        if (cyclePeriods < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cyclePeriods), cyclePeriods,
                "Trend Cycle periods must be greater than or equal to 0 for STC.");
        }

        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                "Fast periods must be greater than 0 for STC.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow periods must be greater than the fast period for STC.");
        }

        // check quotes
        int qtyHistory = quotes.Count();
        int minHistory = Math.Max(2 * (slowPeriods + cyclePeriods), slowPeriods + cyclePeriods + 100);
        if (qtyHistory < minHistory)
        {
            string message = "Insufficient quotes provided for STC.  " +
                string.Format(EnglishCulture,
                "You provided {0} periods of quotes when at least {1} are required.  "
                + "Since this uses a smoothing technique, "
                + "we recommend you use at least {2} data points prior to the intended "
                + "usage date for better precision.", qtyHistory, minHistory, slowPeriods + 250);

            throw new BadQuotesException(nameof(quotes), message);
        }
    }
}
