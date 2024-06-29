using System.Globalization;

namespace Skender.Stock.Indicators;

// QUOTE UTILITIES

public static partial class QuoteUtility
{
    private static readonly CultureInfo NativeCulture = Thread.CurrentThread.CurrentUICulture;

    /* LISTS */

    // convert TQuote type list to built-in Quote type list
    public static IReadOnlyList<Quote> ToQuoteList<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
        => quotes
            .OrderBy(x => x.Timestamp)
            .Select(x => x.ToQuote())
            .ToList();

    // convert TQuote type list to QuoteD type list
    internal static List<QuoteD> ToQuoteD<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
          => quotes
            .OrderBy(x => x.Timestamp)
            .Select(x => x.ToQuoteD())
            .ToList();

    // convert IQuote type list to Reusable list
    public static IReadOnlyList<Reusable> ToReusableList<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote
        => quotes
            .OrderBy(x => x.Timestamp)
            .Select(x => x.ToReusable(candlePart))
            .ToList();

    // convert QuoteD type list to Reusable list
    internal static List<Reusable> ToReusableList(
        this List<QuoteD> qdList,
        CandlePart candlePart)
          => qdList
            .OrderBy(x => x.Timestamp)
            .Select(x => x.ToReusable(candlePart))
            .ToList();

    /* TYPES */

    // convert any IQuote type to native Quote type
    public static Quote ToQuote<TQuote>(this TQuote quote)
        where TQuote : IQuote => new(
            Timestamp: quote.Timestamp,
            Open: quote.Open,
            High: quote.High,
            Low: quote.Low,
            Close: quote.Close,
            Volume: quote.Volume);

    // convert to quote in double precision
    private static QuoteD ToQuoteD(this IQuote quote)
          => new(
            Timestamp: quote.Timestamp,
            Open: (double)quote.Open,
            High: (double)quote.High,
            Low: (double)quote.Low,
            Close: (double)quote.Close,
            Volume: (double)quote.Volume);

    // convert TQuote element to a basic chainable class
    internal static Reusable ToReusable(
        this IQuote q,
        CandlePart candlePart)
        => candlePart switch {

            CandlePart.Open => new(q.Timestamp, (double)q.Open),
            CandlePart.High => new(q.Timestamp, Value: (double)q.High),
            CandlePart.Low => new(q.Timestamp, (double)q.Low),
            CandlePart.Close => new(q.Timestamp, (double)q.Close),
            CandlePart.Volume => new(q.Timestamp, (double)q.Volume),
            CandlePart.Hl2 => new(q.Timestamp, (double)(q.High + q.Low) / 2),
            CandlePart.Hlc3 => new(q.Timestamp, (double)(q.High + q.Low + q.Close) / 3),
            CandlePart.Oc2 => new(q.Timestamp, (double)(q.Open + q.Close) / 2),
            CandlePart.Ohl3 => new(q.Timestamp, (double)(q.Open + q.High + q.Low) / 3),
            CandlePart.Ohlc4 => new(q.Timestamp, (double)(q.Open + q.High + q.Low + q.Close) / 4),

            _ => throw new ArgumentOutOfRangeException(
                nameof(candlePart), candlePart, "Invalid candlePart provided.")
        };

    // convert quoteD element to reusable type
    internal static Reusable ToReusable(
        this QuoteD q,
        CandlePart candlePart)
        => candlePart switch {

            CandlePart.Open => new(q.Timestamp, q.Open),
            CandlePart.High => new(q.Timestamp, q.High),
            CandlePart.Low => new(q.Timestamp, q.Low),
            CandlePart.Close => new(q.Timestamp, q.Close),
            CandlePart.Volume => new(q.Timestamp, q.Volume),
            CandlePart.Hl2 => new(q.Timestamp, (q.High + q.Low) / 2),
            CandlePart.Hlc3 => new(q.Timestamp, (q.High + q.Low + q.Close) / 3),
            CandlePart.Oc2 => new(q.Timestamp, (q.Open + q.Close) / 2),
            CandlePart.Ohl3 => new(q.Timestamp, (q.Open + q.High + q.Low) / 3),
            CandlePart.Ohlc4 => new(q.Timestamp, (q.Open + q.High + q.Low + q.Close) / 4),

            _ => throw new ArgumentOutOfRangeException(
                nameof(candlePart), candlePart, "Invalid candlePart provided.")
        };
}
