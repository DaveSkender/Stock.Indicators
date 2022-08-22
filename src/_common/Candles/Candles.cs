namespace Skender.Stock.Indicators;

public static class Candlesticks
{
    public static IEnumerable<CandleResult> Condense(
        this IEnumerable<CandleResult> candleResults) => candleResults
            .Where(candle => candle.Match != Match.None)
            .ToList();

    public static CandleProperties ToCandle<TQuote>(
        this TQuote quote)
        where TQuote : IQuote => new()
        {
            Date = quote.Date,
            Open = quote.Open,
            High = quote.High,
            Low = quote.Low,
            Close = quote.Close,
            Volume = quote.Volume
        };

    // convert/sort quotes into candles list
    public static IEnumerable<CandleProperties> ToCandles<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
    {
        List<CandleProperties> candlesList = quotes
            .Select(x => x.ToCandle())
            .OrderBy(x => x.Date)
            .ToList();

        // validate
        return candlesList;
    }

    // convert/sort quotes into candle results
    internal static List<CandleResult> ToCandleResults<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
    {
        List<CandleResult> candlesList = quotes
            .Select(x => new CandleResult(x.Date)
            {
                Match = Match.None,
                Candle = x.ToCandle()
            })
            .OrderBy(x => x.Date)
            .ToList();

        // validate
        return candlesList;
    }
}
