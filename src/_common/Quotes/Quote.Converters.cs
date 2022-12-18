using System.Collections.ObjectModel;
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
            .Select(x => x.ToTuple(candlePart));

    // sort quotes
    public static Collection<TQuote> ToSortedCollection<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
    {
        Collection<TQuote> coll = new();
        foreach (TQuote q in quotes.OrderBy(x => x.Date))
        {
            coll.Add(q);
        }

        return coll;
    }

    // TUPLE QUOTES

    // convert quotes to tuple list
    public static Collection<(DateTime, double)> ToTuple<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote
    {
        Collection<(DateTime, double)> coll = new();

        foreach (TQuote q in quotes.OrderBy(x => x.Date))
        {
            coll.Add(q.ToTuple(candlePart));
        }

        return coll;
    }

    // convert tuples to list, with sorting
    public static Collection<(DateTime, double)> ToSortedCollection(
        this IEnumerable<(DateTime date, double value)> tuples)
    {
        Collection<(DateTime, double)> coll = new();

        foreach ((DateTime date, double value) q in tuples.OrderBy(x => x.date))
        {
            coll.Add(q);
        }

        return coll;
    }

    // DOUBLE QUOTES

    // convert to quotes in double precision
    internal static Collection<QuoteD> ToQuoteD<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
    {
        Collection<QuoteD> coll = new();

        foreach (TQuote? q in quotes.OrderBy(x => x.Date))
        {
            coll.Add(new QuoteD
            {
                Date = q.Date,
                Open = (double)q.Open,
                High = (double)q.High,
                Low = (double)q.Low,
                Close = (double)q.Close,
                Volume = (double)q.Volume
            });
        }

        return coll;
    }

    // convert quoteD list to tuples
    internal static Collection<(DateTime, double)> ToTuple(
        this IEnumerable<QuoteD> qdList,
        CandlePart candlePart)
    {
        Collection<(DateTime, double)> coll = new();

        foreach (QuoteD x in qdList.OrderBy(x => x.Date))
        {
            coll.Add(x.ToTuple(candlePart));
        }

        return coll;
    }

    /* ELEMENTS */

    // convert TQuote element to basic tuple
    internal static (DateTime date, double value) ToTuple<TQuote>(
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
    internal static (DateTime, double) ToTuple(
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
