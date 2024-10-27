namespace Skender.Stock.Indicators;

// USE / QUOTE CONVERTER (SERIES)

public static partial class QuoteParts
{
    /// <summary>
    /// Converts <see cref="IReadOnlyList{IQuote}"/> to
    /// an <see cref="IReadOnlyList{QuotePart}"/> list.
    /// </summary>
    /// <remarks>
    /// Use this conversion if indicator needs to
    /// use something other than the default Close price.
    /// </remarks>
    /// <typeparam name="TQuote"></typeparam>
    /// <param name="quotes">Sorted list of IQuote or IReusable items</param>
    /// <param name="candlePart"></param>
    /// <returns>List of IReusable items</returns>
    public static IReadOnlyList<QuotePart> ToQuotePart<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote
    {
        ArgumentNullException.ThrowIfNull(quotes);
        int length = quotes.Count;
        List<QuotePart> result = new(length);

        for (int i = 0; i < length; i++)
        {
            result.Add(quotes[i].ToQuotePart(candlePart));
        }
        return result;
    }

    // QuotePart alias
    /// <inheritdoc cref="ToQuotePart{TQuote}(IReadOnlyList{TQuote}, CandlePart)" />
    public static IReadOnlyList<QuotePart> Use<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote
        => ToQuotePart(quotes, candlePart);

    // TODO: should we deprecate Use in favor of "ToQuotePart"?
    // Probably not, this is a fairly simple alias.
}
