namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Williams %R indicator.
/// </summary>
public static partial class WilliamsR
{
    /// <summary>
    /// Calculates the Williams %R for a series of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A list of WilliamsResult containing the Williams %R values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source is null.</exception>
    public static IReadOnlyList<WilliamsResult> ToWilliamsR(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
        => quotes
            .ToQuoteDList()
            .CalcWilliamsR(lookbackPeriods);

    /// <summary>
    /// Calculates the Williams %R for a series of quotes.
    /// </summary>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A list of WilliamsResult containing the Williams %R values.</returns>
    private static List<WilliamsResult> CalcWilliamsR(
        this List<QuoteD> quotes,
        int lookbackPeriods)
    {
        // check parameter arguments
        Validate(lookbackPeriods);

        // Williams %R is Fast Stochastic Oscillator - 100
        return quotes.CalcStoch(lookbackPeriods, 1, 1, 3, 2, MaType.SMA)
            .ConvertAll(static s => new WilliamsResult(
                Timestamp: s.Timestamp,
                WilliamsR: (s.Oscillator.ToPrecision(14) - 100d).ToPrecision(14)
             ));
    }

    /// <summary>
    /// Creates a buffer list for Williams %R calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A WilliamsRList instance.</returns>
    public static WilliamsRList ToWilliamsRList(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
        => new(lookbackPeriods) { quotes };
}
