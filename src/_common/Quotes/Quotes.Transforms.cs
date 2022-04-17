namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // convert to basic double
    internal static List<Price> ToPrice<TQuote>(
        this IEnumerable<TQuote> quotes, CandlePart element = CandlePart.Close)
        where TQuote : IQuote
    {
        // elements represents the targeted OHLCV parts, so use "O" to return <Open> as base data, etc.
        // convert to basic double precision format
        IEnumerable<Price> prices = element switch
        {
            CandlePart.Open => quotes.Select(x => new Price(x.Date, (double)x.Open)),
            CandlePart.High => quotes.Select(x => new Price(x.Date, (double)x.High)),
            CandlePart.Low => quotes.Select(x => new Price(x.Date, (double)x.Low)),
            CandlePart.Close => quotes.Select(x => new Price(x.Date, (double)x.Close)),
            CandlePart.Volume => quotes.Select(x => new Price(x.Date, (double)x.Volume)),
            CandlePart.HL2 => quotes.Select(x => new Price(x.Date, (double)(x.High + x.Low) / 2)),
            CandlePart.HLC3 => quotes.Select(x => new Price(x.Date, (double)(x.High + x.Low + x.Close) / 3)),
            CandlePart.OC2 => quotes.Select(x => new Price(x.Date, (double)(x.Open + x.Close) / 2)),
            CandlePart.OHL3 => quotes.Select(x => new Price(x.Date, (double)(x.Open + x.High + x.Low) / 3)),
            CandlePart.OHLC4 => quotes.Select(x => new Price(x.Date, (double)(x.Open + x.High + x.Low + x.Close) / 4)),
            _ => new List<Price>()
        };

        return prices.OrderBy(x => x.Date).ToList();
    }
}
