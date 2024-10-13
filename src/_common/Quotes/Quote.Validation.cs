using System.Globalization;

namespace Skender.Stock.Indicators;

// QUOTE UTILITIES

public static partial class Utility
{
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
            // check for duplicates
            if (lastDate == q.Timestamp)
            {
                throw new InvalidQuotesException(
                    string.Format(NativeCulture, "Duplicate date found on {0}.", q.Timestamp));
            }

            lastDate = q.Timestamp;
        }

        return quotesList;
    }

    /// <summary>
    /// Check that quotes are valid and in ascending order.
    /// </summary>
    /// <typeparam name="TQuote">IQuote type</typeparam>
    /// <param name="quotes">List of quotes</param>
    /// <returns>Valid list of quotes</returns>
    /// <exception cref="ArgumentNullException">
    /// List of quotes cannot be a null reference.
    /// </exception>
    /// <exception cref="InvalidQuotesException">
    /// Duplicate or out of sequence quotes found.
    /// </exception>
    public static IReadOnlyList<TQuote> Validate<TQuote>(
        this IReadOnlyList<TQuote> quotes)
        where TQuote : IQuote
    {
        // assumes already sorted
        if (quotes is null)
        {
            throw new ArgumentNullException(nameof(quotes));
        }

        // check for duplicates/sequence
        DateTime lastDate = DateTime.MinValue;
        foreach (TQuote q in quotes)
        {
            // check for duplicates
            if (lastDate == q.Timestamp)
            {
                throw new InvalidQuotesException(
                    string.Format(CultureInfo.InvariantCulture, "Duplicate date found on {0}.", q.Timestamp));
            }

            // check for sequence
            if (lastDate > q.Timestamp)
            {
                throw new InvalidQuotesException(
                    string.Format(NativeCulture, "Quotes are out of sequence on {0}.", q.Timestamp));
            }

            lastDate = q.Timestamp;
        }

        return quotes;
    }

}
