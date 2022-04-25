namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // BASE QUOTE (simplified quote)
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<BaseQuote> GetBaseQuote<TQuote>(
        this IEnumerable<TQuote> quotes, CandlePart element = CandlePart.Close)
        where TQuote : IQuote
    {
        return quotes.ToBaseQuote(element);
    }

    // convert to basic double
    internal static List<BaseQuote> ToBaseQuote<TQuote>(
        this IEnumerable<TQuote> quotes, CandlePart element = CandlePart.Close)
        where TQuote : IQuote
    {
        // elements represents the targeted OHLCV parts, so use "O" to return <Open> as base data, etc.
        // convert to basic double precision format
        IEnumerable<BaseQuote> prices = element switch
        {
            CandlePart.Open => quotes.Select(x => new BaseQuote { Date = x.Date, Value = (double)x.Open }),
            CandlePart.High => quotes.Select(x => new BaseQuote { Date = x.Date, Value = (double)x.High }),
            CandlePart.Low => quotes.Select(x => new BaseQuote { Date = x.Date, Value = (double)x.Low }),
            CandlePart.Close => quotes.Select(x => new BaseQuote { Date = x.Date, Value = (double)x.Close }),
            CandlePart.Volume => quotes.Select(x => new BaseQuote { Date = x.Date, Value = (double)x.Volume }),
            CandlePart.HL2 => quotes.Select(x => new BaseQuote { Date = x.Date, Value = (double)(x.High + x.Low) / 2 }),
            CandlePart.HLC3 => quotes.Select(x => new BaseQuote { Date = x.Date, Value = (double)(x.High + x.Low + x.Close) / 3 }),
            CandlePart.OC2 => quotes.Select(x => new BaseQuote { Date = x.Date, Value = (double)(x.Open + x.Close) / 2 }),
            CandlePart.OHL3 => quotes.Select(x => new BaseQuote { Date = x.Date, Value = (double)(x.Open + x.High + x.Low) / 3 }),
            CandlePart.OHLC4 => quotes.Select(x => new BaseQuote { Date = x.Date, Value = (double)(x.Open + x.High + x.Low + x.Close) / 4 }),
            _ => new List<BaseQuote>()
        };

        return prices.OrderBy(x => x.Date).ToList();
    }
}
