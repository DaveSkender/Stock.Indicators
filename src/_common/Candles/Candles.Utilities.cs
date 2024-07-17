namespace Skender.Stock.Indicators;

public static class CandleUtility
{
    public static IReadOnlyList<CandleResult> Condense(
        this IEnumerable<CandleResult> candleResults) => candleResults
            .Where(candle => candle.Match != Match.None)
            .ToList();

    public static CandleProperties ToCandle<TQuote>(
        this TQuote quote)
        where TQuote : IQuote => new(
            Timestamp: quote.Timestamp,
            Open: quote.Open,
            High: quote.High,
            Low: quote.Low,
            Close: quote.Close,
            Volume: quote.Volume);

    // convert/sort quotes into candles list
    public static IReadOnlyList<CandleProperties> ToCandles<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote => quotes
            .Select(x => x.ToCandle())
            .OrderBy(x => x.Timestamp)
            .ToList();
}
