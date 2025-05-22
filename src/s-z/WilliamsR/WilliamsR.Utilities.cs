namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for the Williams %R indicator.
/// </summary>
public static partial class WilliamsR
{
    /// <summary>
    /// Removes the recommended warmup periods from the Williams %R results.
    /// </summary>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<WilliamsResult> RemoveWarmupPeriods(
        this IReadOnlyList<WilliamsResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.WilliamsR != null);

        return results.Remove(removePeriods);
    }

    // parameter validation
    /// <summary>
    /// Validates the parameters for the Williams %R calculation.
    /// </summary>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the lookback periods are less than or equal to 0.
    /// </exception>
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for William %R.");
        }
    }
}
