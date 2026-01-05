namespace Skender.Stock.Indicators;

public static class UseExtensions
{
    /// <inheritdoc cref="ToQuotePart(IReadOnlyList{IQuote}, CandlePart)"/>
    /// <remarks>This is an alias of <see cref="ToQuotePartList(IReadOnlyList{IQuote}, CandlePart)"/></remarks>
    public static IReadOnlyList<ISeries> Use(
        this IReadOnlyList<IQuote> quotes,
        CandlePart candlePart)
        => quotes.ToQuotePart(candlePart);
}
