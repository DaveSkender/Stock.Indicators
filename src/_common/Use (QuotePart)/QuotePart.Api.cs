namespace Skender.Stock.Indicators;

// QUOTEPART (API)

public static partial class Utility
{
    // SERIES, from Quotes
    /// <summary>
    /// Converts <see cref="IEnumerable{IQuote}"/> to
    /// an <see cref="IReadOnlyList{QuotePart}"/> list.
    /// </summary>
    /// <remarks>
    /// Use this conversion if indicator needs to
    /// use something other than the default Close price.
    /// </remarks>
    /// <typeparam name="TQuote"></typeparam>
    /// <param name="quotes">List of IQuote or IReusable items</param>
    /// <param name="candlePart"></param>
    /// <returns>List of IReusable items</returns>
    public static IReadOnlyList<QuotePart> ToQuotePart<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote
        => quotes
            .OrderBy(q => q.Timestamp)
            .Select(q => q.ToQuotePart(candlePart))
            .ToList();

    // OBSERVER, from Quote Provider
    public static QuotePartHub<TIn> ToQuotePart<TIn>(
        this IQuoteProvider<TIn> quoteProvider,
        CandlePart candlePart)
        where TIn : IQuote
        => new(quoteProvider, candlePart);
}
