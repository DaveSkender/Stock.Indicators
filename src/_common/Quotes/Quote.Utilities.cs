using System.Globalization;

namespace Skender.Stock.Indicators;

// HISTORICAL QUOTES FUNCTIONS (GENERAL)

public static partial class QuoteUtility
{
    private static readonly CultureInfo NativeCulture = Thread.CurrentThread.CurrentUICulture;

    // convert quotes to basic double tuple list
    // validation
    /// <include file='./info.xml' path='info/type[@name="UseCandlePart"]/*' />
    ///
    public static IEnumerable<(DateTime Date, double Value)> Use<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart = CandlePart.Close)
        where TQuote : IQuote => quotes
            .Select(x => x.ToBasicTuple(candlePart));

    internal static List<(DateTime, double)> ToBasicTuple<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart = CandlePart.Close)
        where TQuote : IQuote => quotes
            .Use(candlePart)
            .OrderBy(x => x.Date)
            .ToList();

    internal static List<(DateTime, double)> ToTupleList(
        this IEnumerable<(DateTime date, double value)> quotes)
        => quotes
            .OrderBy(x => x.date)
            .ToList();

    // validation
    /// <include file='./info.xml' path='info/type[@name="Validate"]/*' />
    ///
    public static IEnumerable<TQuote> Validate<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
    {
        // we cannot rely on date consistency when looking back, so we force sort

        List<TQuote> quotesList = quotes.SortToList();

        // check for duplicates
        DateTime lastDate = DateTime.MinValue;
        for (int i = 0; i < quotesList.Count; i++)
        {
            TQuote q = quotesList[i];

            if (lastDate == q.Date)
            {
                throw new InvalidQuotesException(
                    string.Format(NativeCulture, "Duplicate date found on {0}.", q.Date));
            }

            lastDate = q.Date;
        }

        return quotesList;
    }

    // sort quotes
    internal static List<TQuote> SortToList<TQuote>(
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

    // convert quote to basic double tuple
    internal static (DateTime, double) ToBasicTuple<TQuote>(
        this TQuote q, CandlePart candlePart = CandlePart.Close)
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
            _ => throw new ArgumentOutOfRangeException(
                nameof(candlePart), candlePart, "Invalid candlePart provided."),
        };
}
