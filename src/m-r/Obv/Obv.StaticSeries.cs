namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the On-Balance Volume (OBV) indicator.
/// </summary>
public static partial class Obv
{
    /// <summary>
    /// Converts a list of quotes to OBV results.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The list of quotes.</param>
    /// <returns>A list of OBV results.</returns>
    public static IReadOnlyList<ObvResult> ToObv<TQuote>(
        this IReadOnlyList<TQuote> quotes)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcObv();

    /// <summary>
    /// Calculates the OBV for a list of quotes.
    /// </summary>
    /// <param name="source">The list of quotes.</param>
    /// <returns>A list of OBV results.</returns>
    private static List<ObvResult> CalcObv(
        this List<QuoteD> source)
    {
        // initialize
        int length = source.Count;
        List<ObvResult> results = new(length);

        double prevClose = double.NaN;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = source[i];

            ObvResult r = Obv.Increment(
                q.Timestamp,
                q.Close,
                q.Volume,
                prevClose,
                i > 0 ? results[i - 1].Obv : 0);

            results.Add(r);
            prevClose = q.Close;
        }

        return results;
    }
}
