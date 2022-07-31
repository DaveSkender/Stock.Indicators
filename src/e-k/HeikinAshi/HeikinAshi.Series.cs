namespace Skender.Stock.Indicators;

// HEIKIN-ASHI (SERIES)
public static partial class Indicator
{
    internal static List<HeikinAshiResult> CalcHeikinAshi<TQuote>(
        this List<TQuote> quotesList)
        where TQuote : IQuote
    {
        // initialize
        int length = quotesList.Count;
        List<HeikinAshiResult> results = new(length);

        decimal prevOpen = decimal.MinValue;
        decimal prevClose = decimal.MinValue;

        // roll through quotes
        for (int i = 0; i < length; i++)
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

            HeikinAshiResult r = new(q.Date)
            {
                Open = open,
                High = high,
                Low = low,
                Close = close,
                Volume = q.Volume
            };
            results.Add(r);

            // save for next iteration
            prevOpen = open;
            prevClose = close;
        }

        return results;
    }
}
