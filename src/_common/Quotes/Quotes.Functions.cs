using System.Globalization;

namespace Skender.Stock.Indicators;

// HISTORICAL QUOTES FUNCTIONS (GENERAL)

public static class HistoricalQuotes
{
    private static readonly CultureInfo NativeCulture = Thread.CurrentThread.CurrentUICulture;

    // validation
    /// <include file='./info.xml' path='info/type[@name="Validate"]/*' />
    ///
    public static IEnumerable<TQuote> Validate<TQuote>(this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
    {
        // we cannot rely on date consistency when looking back, so we add an index and sort

        List<TQuote> quotesList = quotes.SortToList();

        // check for duplicates
        DateTime lastDate = DateTime.MinValue;
        for (int i = 0; i < quotesList.Count; i++)
        {
            TQuote q = quotesList[i];

            if (lastDate == q.Date)
            {
                throw new BadQuotesException(
                    string.Format(NativeCulture, "Duplicate date found on {0}.", q.Date));
            }

            lastDate = q.Date;
        }

        return quotesList;
    }

    // aggregation (quantization)
    /// <include file='./info.xml' path='info/type[@name="Aggregate"]/*' />
    ///
    public static IEnumerable<Quote> Aggregate<TQuote>(
        this IEnumerable<TQuote> quotes,
        PeriodSize newSize)
        where TQuote : IQuote
    {
        // parameter conversion
        TimeSpan newTimeSpan = newSize.ToTimeSpan();

        // convert
        return quotes.Aggregate(newTimeSpan);
    }

    // aggregation (quantization) using TimeSpan
    /// <include file='./info.xml' path='info/type[@name="AggregateTimeSpan"]/*' />
    ///
    public static IEnumerable<Quote> Aggregate<TQuote>(
        this IEnumerable<TQuote> quotes,
        TimeSpan timeSpan)
        where TQuote : IQuote
    {
        // handle no quotes scenario
        if (quotes == null || !quotes.Any())
        {
            return new List<Quote>();
        }

        if (timeSpan <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeSpan), timeSpan,
                "Historical quotes Aggregation must use a new size value " +
                "that is greater than zero (0).");
        }

        // return aggregation
        return quotes
                .OrderBy(x => x.Date)
                .GroupBy(x => x.Date.RoundDown(timeSpan))
                .Select(x => new Quote
                {
                    Date = x.Key,
                    Open = x.First().Open,
                    High = x.Max(t => t.High),
                    Low = x.Min(t => t.Low),
                    Close = x.Last().Close,
                    Volume = x.Sum(t => t.Volume)
                });
    }

    // sort quotes
    internal static List<TQuote> SortToList<TQuote>(this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
    {
        List<TQuote> quotesList = quotes.OrderBy(x => x.Date).ToList();

        // validate
        return quotesList == null || quotesList.Count == 0
            ? throw new BadQuotesException(nameof(quotes), "No historical quotes provided.")
            : quotesList;
    }

    internal static List<QuoteD> ConvertToList<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
    {
        List<QuoteD> quotesList = quotes
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

        // validate
        return quotesList == null || quotesList.Count == 0
            ? throw new BadQuotesException(nameof(quotes), "No historical quotes provided.")
            : quotesList;
    }

    // convert to basic double
    internal static List<BasicD> ConvertToBasic<TQuote>(
        this IEnumerable<TQuote> quotes, CandlePart element = CandlePart.Close)
        where TQuote : IQuote
    {
        // elements represents the targeted OHLCV parts, so use "O" to return <Open> as base data, etc.
        // convert to basic double precision format
        IEnumerable<BasicD> basicDouble = element switch
        {
            CandlePart.Open => quotes.Select(x => new BasicD { Date = x.Date, Value = (double)x.Open }),
            CandlePart.High => quotes.Select(x => new BasicD { Date = x.Date, Value = (double)x.High }),
            CandlePart.Low => quotes.Select(x => new BasicD { Date = x.Date, Value = (double)x.Low }),
            CandlePart.Close => quotes.Select(x => new BasicD { Date = x.Date, Value = (double)x.Close }),
            CandlePart.Volume => quotes.Select(x => new BasicD { Date = x.Date, Value = (double)x.Volume }),
            CandlePart.HL2 => quotes.Select(x => new BasicD { Date = x.Date, Value = (double)(x.High + x.Low) / 2 }),
            _ => new List<BasicD>(),
        };

        List<BasicD> bdList = basicDouble.OrderBy(x => x.Date).ToList();

        // validate
        return bdList == null || bdList.Count == 0
            ? throw new BadQuotesException(nameof(quotes), "No historical quotes provided.")
            : bdList;
    }
}
