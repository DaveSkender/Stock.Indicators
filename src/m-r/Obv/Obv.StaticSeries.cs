namespace Skender.Stock.Indicators;

/// <summary>
/// On-Balance Volume (OBV) indicator.
/// </summary>
public static partial class Obv
{
    /// <summary>
    /// Converts a list of quotes to OBV results.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <returns>A list of OBV results.</returns>
    public static IReadOnlyList<ObvResult> ToObv(
        this IReadOnlyList<IQuote> quotes)
        => quotes
            .ToQuoteDList()
            .CalcObv();

    /// <summary>
    /// Calculates the OBV for a list of quotes.
    /// </summary>
    /// <param name="quotes">The source list of quotes.</param>
    /// <returns>A list of OBV results.</returns>
    private static List<ObvResult> CalcObv(
        this List<QuoteD> quotes)
    {
        // initialize
        int length = quotes.Count;
        List<ObvResult> results = new(length);

        double prevClose = double.NaN;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = quotes[i];

            ObvResult r = Increment(
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
