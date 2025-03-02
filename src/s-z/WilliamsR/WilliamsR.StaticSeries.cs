namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Williams %R indicator.
/// </summary>
public static partial class WilliamsR
{
    /// <summary>
    /// Calculates the Williams %R for a series of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the elements in the source list, which must implement IQuote.</typeparam>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="lookbackPeriods">The number of lookback periods. Default is 14.</param>
    /// <returns>A list of WilliamsResult containing the Williams %R values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source is null.</exception>
    public static IReadOnlyList<WilliamsResult> ToWilliamsR<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcWilliamsR(lookbackPeriods);

    /// <summary>
    /// Calculates the Williams %R for a series of quotes.
    /// </summary>
    /// <param name="source">The source list of quotes.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <returns>A list of WilliamsResult containing the Williams %R values.</returns>
    private static List<WilliamsResult> CalcWilliamsR(
        this List<QuoteD> source,
        int lookbackPeriods)
    {
        // check parameter arguments
        Validate(lookbackPeriods);

        // convert Fast Stochastic to William %R
        return source.CalcStoch(lookbackPeriods, 1, 1, 3, 2, MaType.SMA)
            .Select(s => new WilliamsResult(
                Timestamp: s.Timestamp,
                WilliamsR: s.Oscillator - 100
             ))
            .ToList();
    }
}
