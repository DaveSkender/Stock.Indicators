/// <summary>
/// Provides utility methods for Rolling Pivot Points calculations.
/// </summary>
public static partial class RollingPivots
{
    /// <summary>
    /// Removes the recommended warmup periods from the Rolling Pivots results.
    /// </summary>
    /// <param name="results">The list of Rolling Pivots results.</param>
    /// <returns>A list of Rolling Pivots results with the warmup periods removed.</returns>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<RollingPivotsResult> RemoveWarmupPeriods(
        this IReadOnlyList<RollingPivotsResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.PP != null);

        return results.Remove(removePeriods);
    }

    /// <summary>
    /// Validates the parameters for Rolling Pivots calculations.
    /// </summary>
    /// <param name="windowPeriods">The number of periods in the rolling window.</param>
    /// <param name="offsetPeriods">The number of periods to offset the window.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are out of range.</exception>
    internal static void Validate(
        int windowPeriods,
        int offsetPeriods)
    {
        // check parameter arguments
        if (windowPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(windowPeriods), windowPeriods,
                "Window periods must be greater than 0 for Rolling Pivot Points.");
        }

        if (offsetPeriods < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offsetPeriods), offsetPeriods,
                "Offset periods must be greater than or equal to 0 for Rolling Pivot Points.");
        }
    }
}
