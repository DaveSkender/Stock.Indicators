namespace Skender.Stock.Indicators;

public static class Candlesticks
{
    public static IEnumerable<CandleResult> Condense(
        this IEnumerable<CandleResult> candleResults) => candleResults
            .Where(candle => candle.Match != Match.None)
            .ToList();

    // convert/sort quotes into candles
    internal static List<CandleResult> ToCandleResults<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
    {
        List<CandleResult> candlesList = quotes
            .Select(x => new CandleResult(x.Date)
            {
                Match = Match.None,
                Candle = new CandleProperties
                {
                    Date = x.Date,
                    Open = x.Open,
                    High = x.High,
                    Low = x.Low,
                    Close = x.Close,
                    Volume = x.Volume
                }
            })
            .OrderBy(x => x.Date)
            .ToList();

        // validate
        return candlesList;
    }
}
