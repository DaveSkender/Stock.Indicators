using System.Collections.ObjectModel;

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
    internal static Collection<CandleResult> ToCandleResults<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
    {
        Collection<CandleResult> candlesList = new();

        foreach (TQuote q in quotes.OrderBy(x => x.Date))
        {
            candlesList.Add(
            new CandleResult(q.Date)
            {
                Match = Match.None,
                Candle = q.ToCandle()
            });
        }

        // validate
        return candlesList;
    }
}
