using System.Collections.ObjectModel;
using System.Globalization;

namespace Skender.Stock.Indicators;

// QUOTE UTILITIES

public static partial class QuoteUtility
{
    private static readonly CultureInfo NativeCulture = Thread.CurrentThread.CurrentUICulture;

    // TUPLE QUOTES

    // convert quotes to tuple list
    public static Collection<(DateTime, double)> ToTupleCollection<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote
        => quotes
            .ToTuple(candlePart)
            .ToCollection();

    internal static List<(DateTime, double)> ToTuple<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote => quotes
            .OrderBy(x => x.Timestamp)
            .Select(x => x.ToTuple(candlePart))
            .ToList();

    // convert tuples to list, with sorting
    public static Collection<(DateTime, double)> ToSortedCollection(
        this IEnumerable<(DateTime date, double value)> tuples)
        => tuples
            .ToSortedList()
            .ToCollection();

    internal static List<(DateTime, double)> ToSortedList(
        this IEnumerable<(DateTime date, double value)> tuples)
        => [.. tuples.OrderBy(x => x.date)];

    // DOUBLE QUOTES

    // convert to quotes in double precision
    internal static QuoteD ToQuoteD<TQuote>(
        this TQuote quote)
        where TQuote : IQuote => new() {
            Timestamp = quote.Timestamp,
            Open = (double)quote.Open,
            High = (double)quote.High,
            Low = (double)quote.Low,
            Close = (double)quote.Close,
            Volume = (double)quote.Volume
        };

    internal static List<QuoteD> ToQuoteD<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote => [.. quotes
            .Select(x => new QuoteD {
                Timestamp = x.Timestamp,
                Open = (double)x.Open,
                High = (double)x.High,
                Low = (double)x.Low,
                Close = (double)x.Close,
                Volume = (double)x.Volume
            })
            .OrderBy(x => x.Timestamp)];

    // convert quoteD list to tuples
    internal static List<(DateTime, double)> ToTuple(
        this List<QuoteD> qdList,
        CandlePart candlePart) => qdList
            .OrderBy(x => x.Timestamp)
            .Select(x => x.ToTuple(candlePart))
            .ToList();

    /* ELEMENTS */

    // convert TQuote element to basic tuple
    internal static (DateTime date, double value) ToTuple<TQuote>(
        this TQuote q,
        CandlePart candlePart)
        where TQuote : IQuote => candlePart switch {
            CandlePart.Open => (q.Timestamp, (double)q.Open),
            CandlePart.High => (q.Timestamp, (double)q.High),
            CandlePart.Low => (q.Timestamp, (double)q.Low),
            CandlePart.Close => (q.Timestamp, (double)q.Close),
            CandlePart.Volume => (q.Timestamp, (double)q.Volume),
            CandlePart.HL2 => (q.Timestamp, (double)(q.High + q.Low) / 2),
            CandlePart.HLC3 => (q.Timestamp, (double)(q.High + q.Low + q.Close) / 3),
            CandlePart.OC2 => (q.Timestamp, (double)(q.Open + q.Close) / 2),
            CandlePart.OHL3 => (q.Timestamp, (double)(q.Open + q.High + q.Low) / 3),
            CandlePart.OHLC4 => (q.Timestamp, (double)(q.Open + q.High + q.Low + q.Close) / 4),
            _ => throw new ArgumentOutOfRangeException(nameof(candlePart), candlePart, "Invalid candlePart provided."),
        };

    // convert TQuote element to basic double class
    internal static BasicResult ToBasicData<TQuote>(
        this TQuote q,
        CandlePart candlePart)
        where TQuote : IQuote => candlePart switch {
            CandlePart.Open => new BasicResult { Timestamp = q.Timestamp, Value = (double)q.Open },
            CandlePart.High => new BasicResult { Timestamp = q.Timestamp, Value = (double)q.High },
            CandlePart.Low => new BasicResult { Timestamp = q.Timestamp, Value = (double)q.Low },
            CandlePart.Close => new BasicResult { Timestamp = q.Timestamp, Value = (double)q.Close },
            CandlePart.Volume => new BasicResult { Timestamp = q.Timestamp, Value = (double)q.Volume },
            CandlePart.HL2 => new BasicResult { Timestamp = q.Timestamp, Value = (double)(q.High + q.Low) / 2 },
            CandlePart.HLC3 => new BasicResult { Timestamp = q.Timestamp, Value = (double)(q.High + q.Low + q.Close) / 3 },
            CandlePart.OC2 => new BasicResult { Timestamp = q.Timestamp, Value = (double)(q.Open + q.Close) / 2 },
            CandlePart.OHL3 => new BasicResult { Timestamp = q.Timestamp, Value = (double)(q.Open + q.High + q.Low) / 3 },
            CandlePart.OHLC4 => new BasicResult { Timestamp = q.Timestamp, Value = (double)(q.Open + q.High + q.Low + q.Close) / 4 },
            _ => throw new ArgumentOutOfRangeException(nameof(candlePart), candlePart, "Invalid candlePart provided."),
        };

    // convert quoteD element to basic tuple
    internal static (DateTime, double) ToTuple(
        this QuoteD q,
        CandlePart candlePart) => candlePart switch {
            CandlePart.Open => (q.Timestamp, q.Open),
            CandlePart.High => (q.Timestamp, q.High),
            CandlePart.Low => (q.Timestamp, q.Low),
            CandlePart.Close => (q.Timestamp, q.Close),
            CandlePart.Volume => (q.Timestamp, q.Volume),
            CandlePart.HL2 => (q.Timestamp, (q.High + q.Low) / 2),
            CandlePart.HLC3 => (q.Timestamp, (q.High + q.Low + q.Close) / 3),
            CandlePart.OC2 => (q.Timestamp, (q.Open + q.Close) / 2),
            CandlePart.OHL3 => (q.Timestamp, (q.Open + q.High + q.Low) / 3),
            CandlePart.OHLC4 => (q.Timestamp, (q.Open + q.High + q.Low + q.Close) / 4),
            _ => throw new ArgumentOutOfRangeException(nameof(candlePart), candlePart, "Invalid candlePart provided."),
        };
}
