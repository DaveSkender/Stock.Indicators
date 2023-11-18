namespace Skender.Stock.Indicators;

// QUOTE UTILITIES

public static partial class QuoteUtility
{
    // VALIDATION
    /// <include file='./info.xml' path='info/type[@name="Validate"]/*' />
    ///
    public static IEnumerable<TQuote> Validate<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
    {
        // we cannot rely on date consistency when looking back, so we force sort

        List<TQuote> quotesList = quotes.ToSortedList();

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

    // COMPARATORS

    // equal quotes
    // note: not using IEquatable IQuote due to user friction
    // also, this is only checking IQuote property values
    // not any user customizations
    internal static bool IsEqual<TQuote>(
        this TQuote a, TQuote b)
        where TQuote : IQuote
        => a.Date == b.Date
        && a.Open == b.Open
        && a.High == b.High
        && a.Low == b.Low
        && a.Close == b.Close
        && a.Volume == b.Volume;

    // equatable tuples
    internal static bool IsEqual(
        this (DateTime date, double value) a,
             (DateTime date, double value) b)
        => a.date == b.date
        && a.value == b.value;
}
