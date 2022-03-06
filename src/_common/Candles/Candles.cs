namespace Skender.Stock.Indicators;

public static class Candlesticks
{
    public static IEnumerable<CandleResult> Condense(
        this IEnumerable<CandleResult> candleResults)
    {
        return candleResults
            .Where(candle => candle.Signal != Signal.None)
            .ToList();
    }

    // convert/sort quotes into candles
    internal static List<CandleResult> ConvertToCandleResults<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
    {
        List<CandleResult> candlesList = quotes
            .Select(x => new CandleResult
            {
                Date = x.Date,
                Signal = Signal.None,
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
