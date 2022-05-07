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
        where TQuote : IQuote => quotes.OrderBy(x => x.Date).ToList();

    // convert to quotes in double precision
    internal static List<QuoteD> ToQuoteD<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote => quotes
            .Select(x => new QuoteD
            {
                Date = x.Date,
                Open = (double?)x.Open,
                High = (double?)x.High,
                Low = (double?)x.Low,
                Close = (double?)x.Close,
                Volume = (double?)x.Volume
                OHLC4 = (double?)x.OHLC4
            })
            .OrderBy(x => x.Date)
            .ToList();
}
