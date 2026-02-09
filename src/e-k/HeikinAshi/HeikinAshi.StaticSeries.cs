namespace Skender.Stock.Indicators;

/// <summary>
/// Heikin-Ashi indicator.
/// </summary>
public static partial class HeikinAshi
{
    /// <summary>
    /// Converts a list of quotes to Heikin-Ashi results.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <returns>A list of Heikin-Ashi results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    public static IReadOnlyList<HeikinAshiResult> ToHeikinAshi(
        this IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        // initialize
        int length = quotes.Count;
        List<HeikinAshiResult> results = new(length);

        decimal prevOpen = decimal.MinValue;
        decimal prevClose = decimal.MinValue;

        if (length > 0)
        {
            IQuote q = quotes[0];
            prevOpen = q.Open;
            prevClose = q.Close;
        }

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            IQuote q = quotes[i];

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
