namespace Skender.Stock.Indicators;

// QUOTEPART TYPE UTILITIES

public static partial class QuoteParts
{
    /// <summary>
    /// convert TQuote element to a basic QuotePart class
    /// </summary>
    /// <param name="q">Quote to convert</param>
    /// <param name="candlePart">The <see cref="CandlePart" /> element.</param>
    /// <returns>Date and value pair</returns>
    internal static QuotePart ToQuotePart(this IQuote q, CandlePart candlePart)
        => new(q.Timestamp, q.ToQuotePartValue(candlePart));

    /// <summary>
    /// convert IQuote to value based on CandlePart
    /// </summary>
    /// <param name="q">Quote to convert</param>
    /// <param name="candlePart">The <see cref="CandlePart" /> element.</param>
    /// <returns>Value of quote part</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a parameter is out of the valid range</exception>
    internal static double ToQuotePartValue(this IQuote q, CandlePart candlePart)

        => candlePart switch {

            CandlePart.Open => (double)q.Open,
            CandlePart.High => (double)q.High,
            CandlePart.Low => (double)q.Low,
            CandlePart.Close => (double)q.Close,
            CandlePart.Volume => (double)q.Volume,
            CandlePart.HL2 => (double)(q.High + q.Low) / 2,
            CandlePart.HLC3 => (double)(q.High + q.Low + q.Close) / 3,
            CandlePart.OC2 => (double)(q.Open + q.Close) / 2,
            CandlePart.OHL3 => (double)(q.Open + q.High + q.Low) / 3,
            CandlePart.OHLC4 => (double)(q.Open + q.High + q.Low + q.Close) / 4,

            _ => throw new ArgumentOutOfRangeException(
                nameof(candlePart), candlePart, "Invalid candlePart provided.")
        };

    /// <summary>
    /// conditional HL2 value if IQuote type
    /// </summary>
    /// <typeparam name="T">Type of record</typeparam>
    /// <param name="item">Item to process</param>
    /// <returns>HL2 value if quote type or reusable value</returns>
    internal static double Hl2OrValue<T>(
        this T item)
        where T : IReusable
        => item.QuotePartOrValue(CandlePart.HL2);

    /// <summary>
    /// conditional CandlePart value if IQuote type
    /// </summary>
    /// <typeparam name="T">Type of record</typeparam>
    /// <param name="item">Item to process</param>
    /// <param name="candlePart">The <see cref="CandlePart" /> element.</param>
    /// <returns>Converted reusable value</returns>
    internal static double QuotePartOrValue<T>(
        this T item, CandlePart candlePart)
        where T : IReusable
        => item is IQuote q
            ? q.ToQuotePartValue(candlePart)
            : item.Value;

    /// <summary>
    /// Uses CandlePart to sort and convert a list from an
    /// <see cref="IQuote"/> to and <see cref="IReusable"/> type.
    /// </summary>
    /// <remarks>
    /// Use this conversion if your source list needs to
    /// conditionally use something other than Close price
    /// as the IReusable value.
    ///<para>
    /// If you provide a list of
    /// IReusable items that are not IQuote, it will simply
    /// cast itself from <typeparamref name="T"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">Type of record</typeparam>
    /// <param name="items">List of IQuote or IReusable items</param>
    /// <param name="candlePart">The <see cref="CandlePart" /> element.</param>
    /// <returns>List of IReusable items</returns>
    internal static IReadOnlyList<IReusable> ToPreferredList<T>(
        this IReadOnlyList<T> items, CandlePart candlePart)
        where T : IReusable
    {
        ArgumentNullException.ThrowIfNull(items);

        return items is IReadOnlyList<IQuote> quotes

            ? quotes.ToQuotePart(candlePart)

            : (IReadOnlyList<IReusable>)items
                .Cast<IReusable>()
                .ToList();
    }
}
