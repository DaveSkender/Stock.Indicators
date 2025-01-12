/// <summary>
/// Utility for candlestick data.
/// </summary>
public static class Candlesticks
{
    /// <summary>
    /// Condenses the list of candle results by filtering out those with no match.
    /// </summary>
    /// <param name="candleResults">The list of candle results to condense.</param>
    /// <returns>A condensed list of candle results.</returns>
    public static IReadOnlyList<CandleResult> Condense(
        this IReadOnlyList<CandleResult> candleResults) => candleResults
            .Where(candle => candle.Match != Match.None)
            .ToList();

    /// <summary>
    /// Converts a quote to candle properties.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quote">The quote to convert.</param>
    /// <returns>The candle properties.</returns>
    public static CandleProperties ToCandle<TQuote>(
        this TQuote quote)
        where TQuote : IQuote => new(
            Timestamp: quote.Timestamp,
            Open: quote.Open,
            High: quote.High,
            Low: quote.Low,
            Close: quote.Close,
            Volume: quote.Volume);

    /// <summary>
    /// Converts and sorts a list of quotes into a list of candle properties.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The list of quotes to convert.</param>
    /// <returns>A sorted list of candle properties.</returns>
    public static IReadOnlyList<CandleProperties> ToCandles<TQuote>(
        this IReadOnlyList<TQuote> quotes)
        where TQuote : IQuote => quotes
            .Select(x => x.ToCandle())
            .OrderBy(x => x.Timestamp)
            .ToList();
}
