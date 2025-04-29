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
    /// <typeparam name = "TQuote" > Configurable Quote type.
    /// See Guide for more information.</typeparam>
    /// <param name = "quotes" > Historical price quotes.</param>
    /// <param name = "maxPriceChangePercent" >
    /// Optional. Maximum absolute percent difference in open and close price.
    /// </param>
    /// <returns>Time series of Doji values.</returns>
    /// <exception cref = "ArgumentOutOfRangeException" >
    /// Invalid parameter value provided.
    /// </exception>
    [Series("DOJI", "Doji", Category.CandlestickPattern, ChartType.Overlay)]
    public static IReadOnlyList<CandleResult> ToDoji<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        [ParamNum<double>("Max Price Change %", 0.1, 0, 0.5)]
        double maxPriceChangePercent = 0.1)
        where TQuote : IQuote
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
            TQuote q = quotes[i];
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
