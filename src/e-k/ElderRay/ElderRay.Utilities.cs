/// <summary>
/// Provides utility methods for Elder Ray calculations.
/// </summary>
public static partial class ElderRay
{
    /// <summary>
    /// Removes the recommended warmup periods from the Elder Ray results.
    /// </summary>
    /// <param name="results">The list of Elder Ray results.</param>
    /// <returns>A list of Elder Ray results with warmup periods removed.</returns>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<ElderRayResult> RemoveWarmupPeriods(
        this IReadOnlyList<ElderRayResult> results)
    {
        int n = results
          .ToList()
          .FindIndex(x => x.BullPower != null) + 1;

        return results.Remove(n + 100);
    }

    /// <summary>
    /// Validates the lookback periods for Elder Ray calculations.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
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
                "Lookback periods must be greater than 0 for Elder-ray Index.");
        }
    }
}
