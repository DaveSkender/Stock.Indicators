using System.Globalization;

namespace Skender.Stock.Indicators;

// HISTORICAL QUOTES FUNCTIONS (GENERAL)

public static partial class HistoricalQuotes
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
        where TQuote : IQuote
    {
        return quotes.OrderBy(x => x.Date).ToList();
    }

    internal static List<QuoteD> ConvertToList<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
    {
        return quotes
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
            CandlePart.HLC3 => quotes.Select(x => new BasicD { Date = x.Date, Value = (double)(x.High + x.Low + x.Close) / 3 }),
            CandlePart.OC2 => quotes.Select(x => new BasicD { Date = x.Date, Value = (double)(x.Open + x.Close) / 2 }),
            CandlePart.OHL3 => quotes.Select(x => new BasicD { Date = x.Date, Value = (double)(x.Open + x.High + x.Low) / 3 }),
            CandlePart.OHLC4 => quotes.Select(x => new BasicD { Date = x.Date, Value = (double)(x.Open + x.High + x.Low + x.Close) / 4 }),
            _ => new List<BasicD>()
        };

        return basicDouble.OrderBy(x => x.Date).ToList();
    }
}
