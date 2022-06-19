using System.Globalization;

namespace Skender.Stock.Indicators;

// HISTORICAL QUOTES FUNCTIONS (GENERAL)

public static partial class QuoteUtility
{
    private static readonly CultureInfo NativeCulture = Thread.CurrentThread.CurrentUICulture;

    /* LISTS */

    // convert TQuotes to basic double tuple list
    /// <include file='./info.xml' path='info/type[@name="UseCandlePart"]/*' />
    ///
    public static IEnumerable<(DateTime Date, double Value)> Use<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart = CandlePart.Close)
        where TQuote : IQuote => quotes
            .Select(x => x.ToBasicTuple(candlePart));

    // sort quotes
    internal static List<TQuote> ToSortedList<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote => quotes
            .OrderBy(x => x.Date)
            .ToList();

    // convert to quotes in double precision
    internal static List<QuoteD> ToQuoteD<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote => quotes
            .Select(x => new QuoteD
            {
                Date = x.Date,
                Open = (double)x.Open,
                High = (double)x.High,
                Low = (double)x.Low,
                Close = (double)x.Close,
                Volume = (double)x.Volume
            })
            .OrderBy(x => x.Date)
            .ToList();

    // convert TQuotes to basic quotes list
    internal static IEnumerable<BasicData> ToBasicData<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote => quotes
            .OrderBy(x => x.Date)
            .Select(q => q.ToBasicData(candlePart));

    // convert quotes to tuple list
    internal static List<(DateTime, double)> ToBasicTuple<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote => quotes
            .OrderBy(x => x.Date)
            .Select(x => x.ToBasicTuple(candlePart))
            .ToList();

    // convert quoteD list to tuples
    internal static List<(DateTime, double)> ToBasicTuple(
        this List<QuoteD> qdList,
        CandlePart candlePart) => qdList
            .OrderBy(x => x.Date)
            .Select(x => x.ToBasicTuple(candlePart))
            .ToList();

    // convert tuples to list, with sorting
    internal static List<(DateTime, double)> ToSortedList(
        this IEnumerable<(DateTime date, double value)> tuples) => tuples
            .OrderBy(x => x.date)
            .ToList();

    /* ELEMENTS */

    // convert TQuote element to basic tuple
    internal static (DateTime, double) ToBasicTuple<TQuote>(
        this TQuote q,
        CandlePart candlePart)
        where TQuote : IQuote => candlePart switch
        {
            CandlePart.Open => (q.Date, (double)q.Open),
            CandlePart.High => (q.Date, (double)q.High),
            CandlePart.Low => (q.Date, (double)q.Low),
            CandlePart.Close => (q.Date, (double)q.Close),
            CandlePart.Volume => (q.Date, (double)q.Volume),
            CandlePart.HL2 => (q.Date, (double)(q.High + q.Low) / 2),
            CandlePart.HLC3 => (q.Date, (double)(q.High + q.Low + q.Close) / 3),
            CandlePart.OC2 => (q.Date, (double)(q.Open + q.Close) / 2),
            CandlePart.OHL3 => (q.Date, (double)(q.Open + q.High + q.Low) / 3),
            CandlePart.OHLC4 => (q.Date, (double)(q.Open + q.High + q.Low + q.Close) / 4),
            _ => throw new ArgumentOutOfRangeException(nameof(candlePart), candlePart, "Invalid candlePart provided."),
        };

    // convert TQuote element to basic double class
    internal static BasicData ToBasicData<TQuote>(
        this TQuote q,
        CandlePart candlePart)
        where TQuote : IQuote => candlePart switch
        {
            CandlePart.Open => new BasicData { Date = q.Date, Value = (double)q.Open },
            CandlePart.High => new BasicData { Date = q.Date, Value = (double)q.High },
            CandlePart.Low => new BasicData { Date = q.Date, Value = (double)q.Low },
            CandlePart.Close => new BasicData { Date = q.Date, Value = (double)q.Close },
            CandlePart.Volume => new BasicData { Date = q.Date, Value = (double)q.Volume },
            CandlePart.HL2 => new BasicData { Date = q.Date, Value = (double)(q.High + q.Low) / 2 },
            CandlePart.HLC3 => new BasicData { Date = q.Date, Value = (double)(q.High + q.Low + q.Close) / 3 },
            CandlePart.OC2 => new BasicData { Date = q.Date, Value = (double)(q.Open + q.Close) / 2 },
            CandlePart.OHL3 => new BasicData { Date = q.Date, Value = (double)(q.Open + q.High + q.Low) / 3 },
            CandlePart.OHLC4 => new BasicData { Date = q.Date, Value = (double)(q.Open + q.High + q.Low + q.Close) / 4 },
            _ => throw new ArgumentOutOfRangeException(nameof(candlePart), candlePart, "Invalid candlePart provided."),
        };

    // convert quoteD element to basic tuple
    internal static (DateTime, double) ToBasicTuple(
        this QuoteD q,
        CandlePart candlePart) => candlePart switch
        {
            CandlePart.Open => (q.Date, q.Open),
            CandlePart.High => (q.Date, q.High),
            CandlePart.Low => (q.Date, q.Low),
            CandlePart.Close => (q.Date, q.Close),
            CandlePart.Volume => (q.Date, q.Volume),
            CandlePart.HL2 => (q.Date, (q.High + q.Low) / 2),
            CandlePart.HLC3 => (q.Date, (q.High + q.Low + q.Close) / 3),
            CandlePart.OC2 => (q.Date, (q.Open + q.Close) / 2),
            CandlePart.OHL3 => (q.Date, (q.Open + q.High + q.Low) / 3),
            CandlePart.OHLC4 => (q.Date, (q.Open + q.High + q.Low + q.Close) / 4),
            _ => throw new ArgumentOutOfRangeException(nameof(candlePart), candlePart, "Invalid candlePart provided."),
        };
}
