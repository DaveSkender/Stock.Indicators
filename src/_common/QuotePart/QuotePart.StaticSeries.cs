namespace Skender.Stock.Indicators;

/// <summary>
/// Provides extension methods for converting quotes to quote parts.
/// </summary>
public static partial class QuoteParts
{
    /// <summary>
    /// Converts list of quotes to a list of <see cref="QuotePart"/>.
    /// <para>
    /// Use this converter if an indicator needs to use something other than the default Close price.
    /// </para>
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="candlePart">The <see cref="CandlePart" /> element.</param>
    /// <returns>List of <see cref="QuotePart"/> records.</returns>
    /// <remarks>This is an alias of <see cref="Use(IReadOnlyList{IQuote}, CandlePart)"/></remarks>
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

    /// <inheritdoc cref="ToQuotePart(IReadOnlyList{IQuote}, CandlePart)"/>
    /// <remarks>This is an alias of <see cref="ToQuotePartList(IReadOnlyList{IQuote}, CandlePart)"/></remarks>
    public static IReadOnlyList<QuotePart> Use(
        this IReadOnlyList<IQuote> quotes,
        CandlePart candlePart)
        => quotes.ToQuotePart(candlePart);

    // TODO: should we deprecate Use in favor of "ToQuotePart"?
    // Probably not, this is a fairly simple alias.
}
