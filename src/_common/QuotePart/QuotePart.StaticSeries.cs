namespace Skender.Stock.Indicators;

/// <summary>
/// Provides extension methods for converting quotes to quote parts.
/// </summary>
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
    /// <param name="quotes">Sorted list of IQuote or IReusable items.</param>
    /// <param name="candlePart">The candle part to convert to.</param>
    /// <returns>List of IReusable items.</returns>
    public static IReadOnlyList<QuotePart> ToQuotePart(
        this IReadOnlyList<IQuote> quotes,
        CandlePart candlePart)
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

    /// <summary>
    /// Converts <see cref="IReadOnlyList{IQuote}"/> to
    /// an <see cref="IReadOnlyList{QuotePart}"/> list.
    /// </summary>
    /// <inheritdoc cref="ToQuotePart{TQuote}(IReadOnlyList{TQuote}, CandlePart)" />
    public static IReadOnlyList<QuotePart> Use(
        this IReadOnlyList<IQuote> quotes,
        CandlePart candlePart)
        => ToQuotePart(quotes, candlePart);

    // TODO: should we deprecate Use in favor of "ToQuotePart"?
    // Probably not, this is a fairly simple alias.
}
