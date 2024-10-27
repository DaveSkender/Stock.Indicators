using System.Globalization;

namespace Skender.Stock.Indicators;

// QUOTE UTILITIES

public static partial class Utility
{
    private static readonly CultureInfo invCulture = CultureInfo.InvariantCulture;

    // VALIDATION
    /// <include file='./info.xml' path='info/type[@name="Validate"]/*' />
    ///
    public static IReadOnlyList<TQuote> Validate<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
    {
        // we cannot rely on date consistency when looking back, so we force sort
        List<TQuote> quotesList = quotes.ToSortedList();

        // check for duplicates
        DateTime lastDate = DateTime.MinValue;
        foreach (TQuote q in quotesList)
        {
            if (lastDate == q.Timestamp)
            {
                throw new InvalidQuotesException(
                    $"Duplicate date found on {q.Timestamp.ToString("o", invCulture)}.");
            }

            lastDate = q.Timestamp;
        }

        return quotesList;
    }
}
