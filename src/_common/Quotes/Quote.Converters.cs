namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for manipulating and handling quote data.
/// </summary>
public static partial class Quotes
{
    /* LISTS */

    /// <summary>
    /// Convert TQuote type list to built-in Quote type list (public API only).
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The list of quotes to convert.</param>
    /// <returns>A list of converted quotes.</returns>
    public static IReadOnlyList<Quote> ToQuoteList<TQuote>(
        this IReadOnlyList<TQuote> quotes)
        where TQuote : IQuote

        => quotes
            .OrderBy(x => x.Timestamp)
            .Select(x => x.ToQuote())
            .ToList();

    /// <summary>
    /// Convert TQuote type list to QuoteD type list.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The list of quotes to convert.</param>
    /// <returns>A list of converted quotes in double precision.</returns>
    internal static List<QuoteD> ToQuoteDList<TQuote>(
        this IReadOnlyList<TQuote> quotes)
        where TQuote : IQuote

          => quotes
            .Select(x => x.ToQuoteD())
            .ToList();

    /* TYPES */

    /// <summary>
    /// Convert any IQuote type to native Quote type (public API only).
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
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
