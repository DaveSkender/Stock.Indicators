namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for the MACD (Moving Average Convergence Divergence) oscillator.
/// </summary>
public static partial class Macd
{
    /// <summary>
    /// Removes the recommended warmup periods from the MACD results.
    /// </summary>
    /// <param name="results">The list of MACD results.</param>
    /// <returns>A list of MACD results with the warmup periods removed.</returns>
    public static IReadOnlyList<MacdResult> RemoveWarmupPeriods(
        this IReadOnlyList<MacdResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Signal != null) + 2;

        return results.Remove(n + 250);
    }

    /// <summary>
    /// Validates the parameters for the MACD calculation.
    /// </summary>
    /// <param name="fastPeriods">The number of periods for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when any of the parameters are out of their valid range.
    /// </exception>
    internal static void Validate(
        int fastPeriods,
        int slowPeriods,
        int signalPeriods)
    {
        IndicatorUtilities.ValidateMultiPeriods(fastPeriods, slowPeriods, signalPeriods, "MACD");
    }
}
