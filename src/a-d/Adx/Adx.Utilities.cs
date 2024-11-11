namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for the ADX (Average Directional Index) indicator.
/// </summary>
public static partial class Adx
{
    /// <summary>
    /// Removes the recommended warmup periods from the ADX results.
    /// </summary>
    /// <param name="results">The list of ADX results.</param>
    /// <returns>A list of ADX results with the warmup periods removed.</returns>
    public static IReadOnlyList<AdxResult> RemoveWarmupPeriods(
        this IReadOnlyList<AdxResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Pdi != null);

        return results.Remove((2 * n) + 100);
    }

    /// <summary>
    /// Increments the ADX value based on the last ADX value and the new price.
    /// </summary>
    /// <param name="lastAdx">The last ADX value.</param>
    /// <param name="newPrice">The new price.</param>
    /// <returns>The incremented ADX value.</returns>
    public static double? Increment(
        double? lastAdx,
        double newPrice)
        => throw new NotImplementedException();

    /// <summary>
    /// Validates the parameters for the ADX calculation.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are less than or equal to 1.</exception>
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for ADX.");
        }
    }
}
