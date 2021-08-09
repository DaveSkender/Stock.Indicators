using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static class Candlesticks
    {
        // sort into candles
        internal static ReadOnlyCollection<Candle> SortCandles<TQuote>(
            this IEnumerable<TQuote> quotes)
            where TQuote : IQuote
        {
            ReadOnlyCollection<Candle> candlesList = quotes
                .Select(x => new Candle
                {
                    Date = x.Date,
                    Open = x.Open,
                    High = x.High,
                    Low = x.Low,
                    Close = x.Close
                })
                .OrderBy(x => x.Date)
                .ToList()
                .AsReadOnly();

            // validate
            return candlesList == null || candlesList.Count == 0
                ? throw new BadQuotesException(nameof(quotes), "No historical quotes provided.")
                : candlesList;
        }
    }
}
