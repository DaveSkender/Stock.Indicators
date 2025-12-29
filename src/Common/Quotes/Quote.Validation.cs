using System.Globalization;

namespace Skender.Stock.Indicators;

// QUOTE UTILITIES: VALIDATION

public static partial class Quotes
{
    private static readonly CultureInfo invariantCulture
        = CultureInfo.InvariantCulture;

    /// <summary>
    /// Check that quotes are valid and in ascending order.
    /// </summary>
    /// <typeparam name="TQuote">IQuote type</typeparam>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
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
                string msg =
                    $"Duplicate date found on {currentDate.ToString("o", invariantCulture)}.";

                throw new InvalidQuotesException(nameof(quotes), msg);
            }

            if (lastDate > currentDate)
            {
                string msg =
                    $"Quotes are out of sequence on {currentDate.ToString("o", invariantCulture)}.";

                throw new InvalidQuotesException(nameof(quotes), msg);
            }

            lastDate = currentDate;
        }

        return quotes;
    }
}
