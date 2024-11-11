namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating True Range (TR) from a list of quotes.
/// </summary>
public static partial class Tr
{
    /// <summary>
    /// Converts a list of quotes to a list of True Range (TR) results.
    /// </summary>
    /// <typeparam name="TQuote">The type of quote.</typeparam>
    /// <param name="quotes">The list of quotes.</param>
    /// <returns>A list of True Range (TR) results.</returns>
    public static IReadOnlyList<TrResult> ToTr<TQuote>(
    this IReadOnlyList<TQuote> quotes)
    where TQuote : IQuote => quotes
        .ToQuoteDList()
        .CalcTr();

    /// <summary>
    /// Calculates the True Range (TR) for a list of quotes.
    /// </summary>
    /// <param name="source">The list of quotes.</param>
    /// <returns>A list of True Range (TR) results.</returns>
    private static List<TrResult> CalcTr(
        this IReadOnlyList<QuoteD> source)
    {
        // initialize
        int length = source.Count;
        TrResult[] results = new TrResult[length];

        // skip first period
        if (length > 0)
        {
            results[0] = new TrResult(source[0].Timestamp, null);
        }

        // roll through source values
        for (int i = 1; i < length; i++)
        {
            QuoteD q = source[i];

            results[i] = new TrResult(
                Timestamp: q.Timestamp,
                Tr: Increment(q.High, q.Low, source[i - 1].Close));
        }

        return new List<TrResult>(results);
    }
}
