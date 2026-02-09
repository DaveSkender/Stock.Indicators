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
    /// Convert IQuote list to QuoteD type list with inline casting.
    /// Uses direct loop instead of LINQ for better performance.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <returns>A list of converted quotes in double precision.</returns>
    /// <exception cref="ArgumentNullException">Thrown when quotes is null.</exception>
    internal static List<QuoteD> ToQuoteDList(
        this IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        int length = quotes.Count;
        List<QuoteD> result = new(length);

        for (int i = 0; i < length; i++)
        {
            IQuote q = quotes[i];
            result.Add(new QuoteD(
                Timestamp: q.Timestamp,
                Open: (double)q.Open,
                High: (double)q.High,
                Low: (double)q.Low,
                Close: (double)q.Close,
                Volume: (double)q.Volume));
        }

        return result;
    }

    /* TYPES */

    /// <summary>
    /// Convert any IQuote type to native Quote type (public API only).
    /// </summary>
    /// <typeparam name="TQuote">Type of quote record</typeparam>
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
