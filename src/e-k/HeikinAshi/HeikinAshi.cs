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
        List<TQuote> quotesList = quotes.ToSortedList();

        // initialize
        List<HeikinAshiResult> results = new(quotesList.Count);

        decimal prevOpen = decimal.MinValue;
        decimal prevClose = decimal.MinValue;

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            TQuote q = quotesList[i];

            // close
            decimal close = (q.Open + q.High + q.Low + q.Close) / 4;

            // open
            decimal open = (prevOpen == decimal.MinValue) ? (q.Open + q.Close) / 2
                : (prevOpen + prevClose) / 2;

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
}
