namespace Skender.Stock.Indicators;

// USE (API)

public static partial class Indicator
{
    // SERIES, from Quotes
    public static IEnumerable<QuotePart> Use<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote
        => quotes.Select(q => q.ToQuotePart(candlePart));

    // OBSERVER, from Quote Provider
    public static Use<TQuote> Use<TQuote>(
        this IQuoteProvider<TQuote> quoteProvider,
        CandlePart candlePart)
        where TQuote : struct, IQuote
        => new(quoteProvider, candlePart);
}
