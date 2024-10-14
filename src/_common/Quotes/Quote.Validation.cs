using System.Globalization;

namespace Skender.Stock.Indicators;

// QUOTE UTILITIES: VALIDATION

public static partial class Utility
{
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
        ArgumentNullException.ThrowIfNull(quotes);

        if (quotes.Count == 0)
        {
            return quotes;
        }

        DateTime lastDate = quotes[0].Timestamp;
        for (int i = 1; i < quotes.Count; i++)
        {
            DateTime currentDate = quotes[i].Timestamp;

            if (lastDate == currentDate)
            {
                throw new InvalidQuotesException(
                    string.Format(CultureInfo.InvariantCulture, "Duplicate date found on {0}.", currentDate));
            }

            if (lastDate > currentDate)
            {
                throw new InvalidQuotesException(
                    string.Format(CultureInfo.InvariantCulture, "Quotes are out of sequence on {0}.", currentDate));
            }

            lastDate = currentDate;
        }

        return quotes;
    }
}
