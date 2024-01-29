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
            .OrderBy(x => x.TickDate)
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
        => tuples
            .OrderBy(x => x.date)
            .ToList();

    // DOUBLE QUOTES

    // convert to quotes in double precision
    internal static QuoteD ToQuoteD<TQuote>(
        this TQuote quote)
        where TQuote : IQuote => new()
        {
            TickDate = quote.TickDate,
            Open = (double)quote.Open,
            High = (double)quote.High,
            Low = (double)quote.Low,
            Close = (double)quote.Close,
            Volume = (double)quote.Volume
        };

    internal static List<QuoteD> ToQuoteD<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote => quotes
            .Select(x => new QuoteD
            {
                TickDate = x.TickDate,
                Open = (double)x.Open,
                High = (double)x.High,
                Low = (double)x.Low,
                Close = (double)x.Close,
                Volume = (double)x.Volume
            })
            .OrderBy(x => x.TickDate)
            .ToList();

    // convert quoteD list to tuples
    internal static List<(DateTime, double)> ToTuple(
        this List<QuoteD> qdList,
        CandlePart candlePart) => qdList
            .OrderBy(x => x.TickDate)
            .Select(x => x.ToTuple(candlePart))
            .ToList();

    /* ELEMENTS */

    // convert TQuote element to basic tuple
    internal static (DateTime date, double value) ToTuple<TQuote>(
        this TQuote q,
        CandlePart candlePart)
        where TQuote : IQuote => candlePart switch
        {
            CandlePart.Open => (q.TickDate, (double)q.Open),
            CandlePart.High => (q.TickDate, (double)q.High),
            CandlePart.Low => (q.TickDate, (double)q.Low),
            CandlePart.Close => (q.TickDate, (double)q.Close),
            CandlePart.Volume => (q.TickDate, (double)q.Volume),
            CandlePart.HL2 => (q.TickDate, (double)(q.High + q.Low) / 2),
            CandlePart.HLC3 => (q.TickDate, (double)(q.High + q.Low + q.Close) / 3),
            CandlePart.OC2 => (q.TickDate, (double)(q.Open + q.Close) / 2),
            CandlePart.OHL3 => (q.TickDate, (double)(q.Open + q.High + q.Low) / 3),
            CandlePart.OHLC4 => (q.TickDate, (double)(q.Open + q.High + q.Low + q.Close) / 4),
            _ => throw new ArgumentOutOfRangeException(nameof(candlePart), candlePart, "Invalid candlePart provided."),
        };

    // convert TQuote element to basic double class
    internal static BasicResult ToBasicData<TQuote>(
        this TQuote q,
        CandlePart candlePart)
        where TQuote : IQuote => candlePart switch
        {
            CandlePart.Open => new BasicResult { TickDate = q.TickDate, Value = (double)q.Open },
            CandlePart.High => new BasicResult { TickDate = q.TickDate, Value = (double)q.High },
            CandlePart.Low => new BasicResult { TickDate = q.TickDate, Value = (double)q.Low },
            CandlePart.Close => new BasicResult { TickDate = q.TickDate, Value = (double)q.Close },
            CandlePart.Volume => new BasicResult { TickDate = q.TickDate, Value = (double)q.Volume },
            CandlePart.HL2 => new BasicResult { TickDate = q.TickDate, Value = (double)(q.High + q.Low) / 2 },
            CandlePart.HLC3 => new BasicResult { TickDate = q.TickDate, Value = (double)(q.High + q.Low + q.Close) / 3 },
            CandlePart.OC2 => new BasicResult { TickDate = q.TickDate, Value = (double)(q.Open + q.Close) / 2 },
            CandlePart.OHL3 => new BasicResult { TickDate = q.TickDate, Value = (double)(q.Open + q.High + q.Low) / 3 },
            CandlePart.OHLC4 => new BasicResult { TickDate = q.TickDate, Value = (double)(q.Open + q.High + q.Low + q.Close) / 4 },
            _ => throw new ArgumentOutOfRangeException(nameof(candlePart), candlePart, "Invalid candlePart provided."),
        };

    // convert quoteD element to basic tuple
    internal static (DateTime, double) ToTuple(
        this QuoteD q,
        CandlePart candlePart) => candlePart switch
        {
            CandlePart.Open => (q.TickDate, q.Open),
            CandlePart.High => (q.TickDate, q.High),
            CandlePart.Low => (q.TickDate, q.Low),
            CandlePart.Close => (q.TickDate, q.Close),
            CandlePart.Volume => (q.TickDate, q.Volume),
            CandlePart.HL2 => (q.TickDate, (q.High + q.Low) / 2),
            CandlePart.HLC3 => (q.TickDate, (q.High + q.Low + q.Close) / 3),
            CandlePart.OC2 => (q.TickDate, (q.Open + q.Close) / 2),
            CandlePart.OHL3 => (q.TickDate, (q.Open + q.High + q.Low) / 3),
            CandlePart.OHLC4 => (q.TickDate, (q.Open + q.High + q.Low + q.Close) / 4),
            _ => throw new ArgumentOutOfRangeException(nameof(candlePart), candlePart, "Invalid candlePart provided."),
        };
}
