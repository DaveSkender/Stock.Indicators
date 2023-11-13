namespace Skender.Stock.Indicators;

// USE (API)

public static partial class QuoteUtility
{
    // convert TQuotes to basic double tuple list
    /// <include file='./info.xml' path='info/type[@name="UseCandlePart"]/*' />
    ///
    public static IEnumerable<(DateTime Date, double Value)> Use<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart = CandlePart.Close)
        where TQuote : IQuote => quotes
            .Select(x => x.ToTuple(candlePart));

    // OBSERVER, from Quote Provider
    public static Use Use(
        this QuoteProvider provider,
        CandlePart candlePart = CandlePart.Close)
            => new(provider, candlePart);
}
