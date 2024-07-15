namespace Skender.Stock.Indicators;

// USE (API)

public static partial class QuotePartUtility
{
    // SERIES, from Quotes
    public static IEnumerable<QuotePart> Use(
        this IEnumerable<IQuote> quotes,
        CandlePart candlePart)
        => quotes
            .OrderBy(q => q.Timestamp)
            .Select(q => q.ToQuotePart(candlePart));

    // SERIES, from Quotes
    public static IEnumerable<QuotePart> Use<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote
        => quotes
            .OrderBy(q => q.Timestamp)
            .Select(q => q.ToQuotePart(candlePart));

    // OBSERVER, from Quote Provider
    public static QuotePartHub<TIn> ToQuotePart<TIn>(
        this IQuoteProvider<TIn> quoteProvider,
        CandlePart candlePart)
        where TIn : IQuote
        => new(quoteProvider, candlePart);
}
