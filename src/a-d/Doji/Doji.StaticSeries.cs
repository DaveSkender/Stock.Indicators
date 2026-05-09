namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for identifying Doji candlestick patterns in a series of quotes.
/// </summary>
public static partial class Doji
{
    /// <summary>
    /// Doji is a single candlestick pattern where open and close price
    /// are virtually identical, representing market indecision.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="maxPriceChangePercent">
    /// Optional. Maximum absolute percent difference in open and close price.
    /// </param>
    /// <returns>Time series of Doji values.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Invalid parameter value provided.
    /// </exception>
    public static IReadOnlyList<CandleResult> ToDoji(
        this IReadOnlyList<IQuote> quotes,
        double maxPriceChangePercent = 0.1)
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(quotes);
        Validate(maxPriceChangePercent);

        // initialize
        int length = quotes.Count;
        List<CandleResult> results = new(length);

        maxPriceChangePercent /= 100;

        // roll through candles
        for (int i = 0; i < length; i++)
        {
            IQuote q = quotes[i];
            decimal? matchPrice = null;
            Match matchType = Match.None;

            // check for current signal
            if (q.Open != 0
                && Math.Abs((double)(q.Close / q.Open) - 1d) <= maxPriceChangePercent)
            {
                matchPrice = q.Close;
                matchType = Match.Neutral;
            }

            results.Add(new CandleResult(
                timestamp: q.Timestamp,
                quote: q,
                match: matchType,
                price: matchPrice));
        }

        return results;
    }
}
