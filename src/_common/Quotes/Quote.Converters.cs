namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for manipulating and handling quote data.
/// </summary>
public static partial class Quotes
{
    /* LISTS */

    /// <summary>
    /// Convert IQuote list to built-in Quote type list (public API only).
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <returns>A list of converted quotes.</returns>
    public static IReadOnlyList<Quote> ToQuoteList(
        this IReadOnlyList<IQuote> quotes)

        => quotes
            .OrderBy(static x => x.Timestamp)
            .Select(static x => x.ToQuote())
            .ToList();

    /// <summary>
    /// Convert IQuote list to QuoteD type list.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <returns>A list of converted quotes in double precision.</returns>
    internal static List<QuoteD> ToQuoteDList(
        this IReadOnlyList<IQuote> quotes)

          => quotes
            .Select(static x => x.ToQuoteD())
            .ToList();

    /* TYPES */

    /// <summary>
    /// Convert any IQuote type to native Quote type (public API only).
    /// </summary>
    /// <typeparam name="TQuote"></typeparam>
    /// <param name="quote">The quote to convert.</param>
    /// <returns>A converted quote.</returns>
    public static Quote ToQuote<TQuote>(this TQuote quote)
        where TQuote : IQuote

        => new(
            Timestamp: quote.Timestamp,
            Open: quote.Open,
            High: quote.High,
            Low: quote.Low,
            Close: quote.Close,
            Volume: quote.Volume);

    /// <summary>
    /// Convert to quote in double precision.
    /// </summary>
    /// <param name="quote">The quote to convert.</param>
    /// <returns>A converted quote in double precision.</returns>
    internal static QuoteD ToQuoteD(this IQuote quote)

        => new(
            Timestamp: quote.Timestamp,
            Open: (double)quote.Open,
            High: (double)quote.High,
            Low: (double)quote.Low,
            Close: (double)quote.Close,
            Volume: (double)quote.Volume);
}
