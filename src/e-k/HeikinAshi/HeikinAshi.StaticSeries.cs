namespace Skender.Stock.Indicators;

// HEIKIN-ASHI (SERIES)

public static class HeikinAshi
{
    public static IReadOnlyList<HeikinAshiResult> ToHeikinAshi<TQuote>(
        this IReadOnlyList<TQuote> quotes)
        where TQuote : IQuote
    {
        ArgumentNullException.ThrowIfNull(quotes);

        // initialize
        int length = quotes.Count;
        List<HeikinAshiResult> results = new(length);

        decimal prevOpen = decimal.MinValue;
        decimal prevClose = decimal.MinValue;

        if (length > 0)
        {
            TQuote q = quotes[0];
            prevOpen = q.Open;
            prevClose = q.Close;
        }

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            TQuote q = quotes[i];

            // close
            decimal close = (q.Open + q.High + q.Low + q.Close) / 4;

            // open
            decimal open = (prevOpen + prevClose) / 2;

            // high
            decimal[] arrH = [q.High, open, close];
            decimal high = arrH.Max();

            // low
            decimal[] arrL = [q.Low, open, close];
            decimal low = arrL.Min();

            results.Add(new HeikinAshiResult(
                Timestamp: q.Timestamp,
                Open: open,
                High: high,
                Low: low,
                Close: close,
                Volume: q.Volume));

            // save for next iteration
            prevOpen = open;
            prevClose = close;
        }

        return results;
    }
}
