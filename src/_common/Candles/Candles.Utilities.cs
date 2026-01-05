namespace Skender.Stock.Indicators;

/// <summary>
/// Utility for candlestick data.
/// </summary>
public static class Candlesticks
{
    /// <summary>
    /// Converts Candle results to a chainable list using the Price field.
    /// </summary>
    /// <param name="results">The list of Candle results.</param>
    /// <returns>A list of chainable values.</returns>
    public static IReadOnlyList<QuotePart> Use(
        this IReadOnlyList<CandleResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);
        int length = results.Count;
        List<QuotePart> list = new(length);

        for (int i = 0; i < length; i++)
        {
            CandleResult r = results[i];
            double value = (double?)r.Price ?? double.NaN;
            list.Add(new QuotePart(r.Timestamp, value));
        }

        return list;
    }

    /// <summary>
    /// Condenses the list of candle results by filtering out those with no match.
    /// </summary>
    /// <param name="candleResults">The list of candle results to condense.</param>
    /// <returns>A condensed list of candle results.</returns>
    public static IReadOnlyList<CandleResult> Condense(
        this IReadOnlyList<CandleResult> candleResults) => candleResults
            .Where(static candle => candle.Match != Match.None)
            .ToList();

    /// <summary>
    /// Converts a quote to candle properties.
    /// </summary>
    /// <typeparam name="TQuote">Type of quote record</typeparam>
    /// <param name="quote">The quote to convert.</param>
    /// <returns>The candle properties.</returns>
    public static CandleProperties ToCandle<TQuote>(
        this TQuote quote)
        where TQuote : IQuote
        => new(
            Timestamp: quote.Timestamp,
            Open: quote.Open,
            High: quote.High,
            Low: quote.Low,
            Close: quote.Close,
            Volume: quote.Volume);

    /// <summary>
    /// Converts and sorts a list of quotes into a list of candle properties.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <returns>A sorted list of candle properties.</returns>
    public static IReadOnlyList<CandleProperties> ToCandles(
        this IReadOnlyList<IQuote> quotes)
        => quotes
            .Select(static x => x.ToCandle())
            .OrderBy(static x => x.Timestamp)
            .ToList();
}
