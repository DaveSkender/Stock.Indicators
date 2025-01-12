/// <summary>
/// Provides utility methods for Endpoint Moving Average (EPMA) calculations.
/// </summary>
public static partial class Epma
{
    /// <summary>
    /// Removes the recommended warmup periods from the EPMA results.
    /// </summary>
    /// <param name="results">The list of EPMA results.</param>
    /// <returns>A list of EPMA results with warmup periods removed.</returns>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<EpmaResult> RemoveWarmupPeriods(
        this IReadOnlyList<EpmaResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.Epma != null);

        return results.Remove(removePeriods);
    }

    /// <summary>
    /// Validates the lookback periods for EPMA calculations.
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
                "Lookback periods must be greater than 0 for Epma.");
        }
    }
}
