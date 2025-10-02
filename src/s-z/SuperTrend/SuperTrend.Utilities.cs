namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for the SuperTrend indicator.
/// </summary>
public static partial class SuperTrend
{
    /// <summary>
    /// Removes empty (null) periods from the results.
    /// </summary>
    /// <param name="results">The list of SuperTrend results.</param>
    /// <returns>A condensed list of SuperTrend results without null periods.</returns>
    public static IReadOnlyList<SuperTrendResult> Condense(
        this IReadOnlyList<SuperTrendResult> results)
    {
        List<SuperTrendResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.SuperTrend is null);

        return resultsList.ToSortedList();
    }

    /// <summary>
    /// Removes the recommended warmup periods from the results.
    /// </summary>
    /// <param name="results">The list of SuperTrend results.</param>
    /// <returns>A list of SuperTrend results with warmup periods removed.</returns>
    public static IReadOnlyList<SuperTrendResult> RemoveWarmupPeriods(
        this IReadOnlyList<SuperTrendResult> results)
    {
        int removePeriods = results
            .FindIndex(x => x.SuperTrend != null);

        return results.Remove(removePeriods);
    }

    /// <summary>
    /// Validates the parameters for SuperTrend calculation.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods for the lookback.</param>
    /// <param name="multiplier">The multiplier for the SuperTrend calculation.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a parameter is out of range.</exception>
    internal static void Validate(
        int lookbackPeriods,
        double multiplier)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for SuperTrend.");
        }

        if (multiplier <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(multiplier), multiplier,
                "Multiplier must be greater than 0 for SuperTrend.");
        }
    }
}
