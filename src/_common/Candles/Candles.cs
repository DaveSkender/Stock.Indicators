namespace Skender.Stock.Indicators;

public static class Candlesticks
{
    // convert/sort quotes into candles
    internal static List<Candle> ConvertToCandles<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
    {
        List<Candle> candlesList = quotes
            .Select(x => new Candle
            {
                Date = x.Date,
                Open = x.Open,
                High = x.High,
                Low = x.Low,
                Close = x.Close
            })
            .OrderBy(x => x.Date)
            .ToList();

        // validate
        return candlesList;
    }
}
