namespace Skender.Stock.Indicators;

// USE (API)

public static partial class Indicator
{
    // SERIES, from Quotes
    /// <include file='./info.xml' path='info/type[@name="standard"]/*' />
    ///
    public static IEnumerable<(DateTime Timestamp, double Value)> Use<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote
        => quotes.Select(x => x.ToTuple(candlePart));

    // OBSERVER, from Quote Provider
    public static Use<TQuote> Use<TQuote>(
        this QuoteProvider<TQuote> quoteProvider,
        CandlePart candlePart)
        where TQuote : IQuote, new()
        => new(quoteProvider, candlePart);
}
