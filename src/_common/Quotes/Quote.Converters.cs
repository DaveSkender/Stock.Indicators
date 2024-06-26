using System.Globalization;

namespace Skender.Stock.Indicators;

// QUOTE UTILITIES

public static partial class QuoteUtility
{
    private static readonly CultureInfo NativeCulture = Thread.CurrentThread.CurrentUICulture;

    // DOUBLE QUOTES

    // convert to quotes in double precision
    internal static QuoteD ToQuoteD<TQuote>(
        this TQuote quote)
        where TQuote : IQuote => new(
            Timestamp: quote.Timestamp,
            Open: (double)quote.Open,
            High: (double)quote.High,
            Low: (double)quote.Low,
            Close: (double)quote.Close,
            Volume: (double)quote.Volume);

    internal static List<QuoteD> ToQuoteD<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote => quotes
            .Select(x => new QuoteD(
                Timestamp: x.Timestamp,
                Open: (double)x.Open,
                High: (double)x.High,
                Low: (double)x.Low,
                Close: (double)x.Close,
                Volume: (double)x.Volume))
            .OrderBy(x => x.Timestamp)
            .ToList();

    // convert quoteD type list to reusable list
    internal static List<Reusable> ToReusableList(
        this List<QuoteD> qdList,
        CandlePart candlePart)
          => qdList
            .OrderBy(x => x.Timestamp)
            .Select(x => x.ToReusable(candlePart))
            .ToList();

    // convert IQuote type list to reusable list
    internal static List<Reusable> ToReusableList<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote
          => quotes
            .OrderBy(x => x.Timestamp)
            .Select(x => x.ToReusable(candlePart))
            .ToList();

    /* ELEMENTS */

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
            CandlePart.HL2 => new(q.Timestamp, (double)(q.High + q.Low) / 2),
            CandlePart.HLC3 => new(q.Timestamp, (double)(q.High + q.Low + q.Close) / 3),
            CandlePart.OC2 => new(q.Timestamp, (double)(q.Open + q.Close) / 2),
            CandlePart.OHL3 => new(q.Timestamp, (double)(q.Open + q.High + q.Low) / 3),
            CandlePart.OHLC4 => new(q.Timestamp, (double)(q.Open + q.High + q.Low + q.Close) / 4),

            _ => throw new ArgumentOutOfRangeException(
                nameof(candlePart), candlePart, "Invalid candlePart provided."),
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
            CandlePart.HL2 => new(q.Timestamp, (q.High + q.Low) / 2),
            CandlePart.HLC3 => new(q.Timestamp, (q.High + q.Low + q.Close) / 3),
            CandlePart.OC2 => new(q.Timestamp, (q.Open + q.Close) / 2),
            CandlePart.OHL3 => new(q.Timestamp, (q.Open + q.High + q.Low) / 3),
            CandlePart.OHLC4 => new(q.Timestamp, (q.Open + q.High + q.Low + q.Close) / 4),

            _ => throw new ArgumentOutOfRangeException(
                nameof(candlePart), candlePart, "Invalid candlePart provided."),
        };
}
