namespace Skender.Stock.Indicators;

// USE (API)

public static partial class Utility
{
    // SERIES, from Quotes
    public static IReadOnlyList<QuotePart> Use<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote
        => quotes
            .OrderBy(q => q.Timestamp)
            .Select(q => q.ToQuotePart(candlePart))
            .ToList();

    // SERIES, from Quotes (QuotePart alias)
    /// <inheritdoc cref="Use{TQuote}(IEnumerable{TQuote}, CandlePart)"/>
    public static IReadOnlyList<QuotePart> GetQuotePart<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote
        => quotes
            .OrderBy(q => q.Timestamp)
            .Select(q => q.ToQuotePart(candlePart))
            .ToList();
}
