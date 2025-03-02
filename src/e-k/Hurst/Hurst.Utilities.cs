namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for Hurst Exponent calculations.
/// </summary>
public static partial class Hurst
{
    /// <summary>
    /// Removes the recommended warmup periods from the Hurst Exponent results.
    /// </summary>
    /// <param name="results">The list of Hurst Exponent results.</param>
    /// <returns>A list of Hurst Exponent results with the warmup periods removed.</returns>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<HurstResult> RemoveWarmupPeriods(
        this IReadOnlyList<HurstResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.HurstExponent != null);

        return results.Remove(removePeriods);
    }

    /// <summary>
    /// Validates the lookback periods for Hurst Exponent calculations.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the lookback periods are less than 20.
    /// </exception>
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods < 20)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be at least 20 for Hurst Exponent.");
        }
    }
}
