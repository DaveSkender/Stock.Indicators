using System.ComponentModel;

namespace Skender.Stock.Indicators;

// QUOTE UTILITIES (CONVERTERS)

public static partial class Quotes
{
    /* LISTS */

    // convert TQuote type list to built-in Quote type list
    [Category("Public API only")]
    public static IReadOnlyList<Quote> ToQuoteList<TQuote>(
        this IReadOnlyList<TQuote> quotes)
        where TQuote : IQuote

        => quotes
            .OrderBy(x => x.Timestamp)
            .Select(x => x.ToQuote())
            .ToList();

    // convert TQuote type list to QuoteD type list
    internal static List<QuoteD> ToQuoteDList<TQuote>(
        this IReadOnlyList<TQuote> quotes)
        where TQuote : IQuote

          => quotes
            .Select(x => x.ToQuoteD())
            .ToList();

    /* TYPES */

    // convert any IQuote type to native Quote type
    [Category("Public API only")]
    public static Quote ToQuote<TQuote>(this TQuote quote)
        where TQuote : IQuote

        => new(
            Timestamp: quote.Timestamp,
            Open: quote.Open,
            High: quote.High,
            Low: quote.Low,
            Close: quote.Close,
            Volume: quote.Volume);

    // convert to quote in double precision
    internal static QuoteD ToQuoteD(this IQuote quote)

        => new(
            Timestamp: quote.Timestamp,
            Open: (double)quote.Open,
            High: (double)quote.High,
            Low: (double)quote.Low,
            Close: (double)quote.Close,
            Volume: (double)quote.Volume);
}
