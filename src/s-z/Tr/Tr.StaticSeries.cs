namespace Skender.Stock.Indicators;

/// <summary>
/// True Range (TR) from a list of quotes indicator.
/// </summary>
public static partial class Tr
{
    /// <summary>
    /// Converts a list of quotes to a list of True Range (TR) results.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <returns>A list of True Range (TR) results.</returns>
    public static IReadOnlyList<TrResult> ToTr(
    this IReadOnlyList<IQuote> quotes)
    => quotes
        .ToQuoteDList()
        .CalcTr();

    /// <summary>
    /// Calculates the True Range (TR) for a list of quotes.
    /// </summary>
    /// <param name="quotes">The source list of quotes.</param>
    /// <returns>A list of True Range (TR) results.</returns>
    private static List<TrResult> CalcTr(
        this List<QuoteD> quotes)
    {
        // initialize
        int length = quotes.Count;
        TrResult[] results = new TrResult[length];

        // skip first period
        if (length > 0)
        {
            results[0] = new TrResult(quotes[0].Timestamp, null);
        }

        // roll through source values
        for (int i = 1; i < length; i++)
        {
            QuoteD q = quotes[i];

            results[i] = new TrResult(
                Timestamp: q.Timestamp,
                Tr: Increment(q.High, q.Low, quotes[i - 1].Close));
        }

        return [.. results];
    }
}
