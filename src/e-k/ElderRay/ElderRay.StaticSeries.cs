namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Elder Ray indicator.
/// </summary>
public static partial class ElderRay
{
    /// <summary>
    /// Converts a list of quotes to Elder Ray results.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote data.</typeparam>
    /// <param name="quotes">The list of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A list of Elder Ray results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static IReadOnlyList<ElderRayResult> ToElderRay<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 13)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcElderRay(lookbackPeriods);

    /// <summary>
    /// Calculates the Elder Ray indicator.
    /// </summary>
    /// <param name="source">The list of source data.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A list of Elder Ray results.</returns>
    private static List<ElderRayResult> CalcElderRay(
        this IReadOnlyList<QuoteD> source,
        int lookbackPeriods)
    {
        // check parameter arguments
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<ElderRayResult> results = new(length);

        // EMA
        IReadOnlyList<EmaResult> emaResults
            = source.ToEma(lookbackPeriods);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = source[i];
            EmaResult e = emaResults[i];

            results.Add(new(
                Timestamp: e.Timestamp,
                Ema: e.Ema,
                BullPower: q.High - e.Ema,
                BearPower: q.Low - e.Ema));
        }

        return results;
    }
}
