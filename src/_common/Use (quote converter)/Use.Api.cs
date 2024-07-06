namespace Skender.Stock.Indicators;

// USE (API)

public static partial class Indicator
{
    // SERIES, from Quotes
    public static IEnumerable<Reusable> Use(
        this IEnumerable<IQuote> quotes,
        CandlePart candlePart)
        => quotes
            .OrderBy(q => q.Timestamp)
            .Select(q => q.ToReusable(candlePart));

    // SERIES, from Quotes
    public static IEnumerable<Reusable> Use<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote
        => quotes
            .OrderBy(q => q.Timestamp)
            .Select(q => q.ToReusable(candlePart));

    // OBSERVER, from Quote Provider
    public static Use<TQuote> Use<TQuote>(
        this QuoteProvider<TQuote> quoteProvider,
        CandlePart candlePart)
        where TQuote : struct, IQuote
        => new(quoteProvider, candlePart);
}
