namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // HEIKIN-ASHI
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<HeikinAshiResult> GetHeikinAshi<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
    {
        // sort quotes
        List<TQuote> quotesList = quotes.SortToList();

        // check parameter arguments
        ValidateHeikinAshi(quotes);

        // initialize
        List<HeikinAshiResult> results = new(quotesList.Count);

        decimal? prevOpen = null;
        decimal? prevClose = null;

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            TQuote q = quotesList[i];

            // close
            decimal close = (q.Open + q.High + q.Low + q.Close) / 4;

            // open
            decimal open = (prevOpen == null) ? (q.Open + q.Close) / 2
                : (decimal)(prevOpen + prevClose) / 2;

            // high
            decimal[] arrH = { q.High, open, close };
            decimal high = arrH.Max();

            // low
            decimal[] arrL = { q.Low, open, close };
            decimal low = arrL.Min();

            HeikinAshiResult result = new()
            {
                Date = q.Date,
                Open = open,
                High = high,
                Low = low,
                Close = close,
                Volume = q.Volume
            };
            results.Add(result);

            // save for next iteration
            prevOpen = open;
            prevClose = close;
        }

        return results;
    }

    // convert to quotes
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Convert"]/*' />
    ///
    public static IEnumerable<Quote> ConvertToQuotes(
        this IEnumerable<HeikinAshiResult> results)
    {
        return results
          .Select(x => new Quote
          {
              Date = x.Date,
              Open = x.Open,
              High = x.High,
              Low = x.Low,
              Close = x.Close,
              Volume = x.Volume
          })
          .ToList();
    }

    // parameter validation
    private static void ValidateHeikinAshi<TQuote>(
        IEnumerable<TQuote> quotes)
        where TQuote : IQuote
    {
        // check quotes
        int qtyHistory = quotes.Count();
        int minHistory = 2;
        if (config.UseBadQuotesException && qtyHistory < minHistory)
        {
            string message = "Insufficient quotes provided for Heikin-Ashi.  " +
                string.Format(
                    EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

            throw new BadQuotesException(nameof(quotes), message);
        }
    }
}
