namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for the Volatility Stop indicator.
/// </summary>
public static partial class VolatilityStop
{
    // remove recommended periods
    /// <summary>
    /// Removes the warmup periods from the Volatility Stop results.
    /// </summary>
    /// <param name="results">The list of Volatility Stop results.</param>
    /// <returns>A list of Volatility Stop results with the warmup periods removed.</returns>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<VolatilityStopResult> RemoveWarmupPeriods(
        this IReadOnlyList<VolatilityStopResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Sar != null);

        removePeriods = Math.Max(100, removePeriods);

        return results.Remove(removePeriods);
    }

    // parameter validation
    /// <summary>
    /// Validates the parameters for the Volatility Stop calculation.
    /// </summary>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <param name="multiplier">The multiplier for the Average True Range.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the lookback periods are less than or equal to 1, or when the multiplier is less than or equal to 0.
    /// </exception>
    internal static void Validate(
        int lookbackPeriods,
        double multiplier)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for Volatility Stop.");
        }

        if (multiplier <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(multiplier), multiplier,
                "ATR Multiplier must be greater than 0 for Volatility Stop.");
        }
    }
}
