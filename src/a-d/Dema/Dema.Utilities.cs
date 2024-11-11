namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for the DEMA (Double Exponential Moving Average) indicator.
/// </summary>
public static partial class Dema
{
    /// <summary>
    /// Removes the recommended warmup periods from the DEMA results.
    /// </summary>
    /// <param name="results">The list of DEMA results.</param>
    /// <returns>A list of DEMA results with the warmup periods removed.</returns>
    public static IReadOnlyList<DemaResult> RemoveWarmupPeriods(
        this IReadOnlyList<DemaResult> results)
    {
        int n = results
          .ToList()
          .FindIndex(x => x.Dema != null) + 1;

        return results.Remove(2 * n + 100);
    }

    /// <summary>
    /// Validates the parameters for the DEMA calculation.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are less than or equal to 0.</exception>
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for DEMA.");
        }
    }
}
