namespace Skender.Stock.Indicators;

// USE (API)

public static partial class QuoteUtility  // TODO: is this an appropriate class name?
{
    // convert TQuotes to basic double tuple list
    /// <include file='./info.xml' path='info/type[@name="UseCandlePart"]/*' />
    ///
    public static IEnumerable<(DateTime Date, double Value)> Use<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart = CandlePart.Close)
        where TQuote : IQuote, new() => quotes
            .Select(x => x.ToTuple(candlePart));

    // OBSERVER, from Quote Provider
    public static Use<TQuote> Use<TQuote>(
        this QuoteProvider<TQuote> provider,
        CandlePart candlePart = CandlePart.Close)
        where TQuote : IQuote, new()
            => new(provider, candlePart);
}
