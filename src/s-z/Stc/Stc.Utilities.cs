namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for Schaff Trend Cycle (STC) calculations.
/// </summary>
public static partial class Stc
{
    /// <summary>
    /// Removes the recommended warmup periods from the results.
    /// </summary>
    /// <param name="results">The list of STC results.</param>
    /// <returns>A list of STC results with warmup periods removed.</returns>
    public static IReadOnlyList<StcResult> RemoveWarmupPeriods(
        this IReadOnlyList<StcResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Stc != null);

        return results.Remove(n + 250);
    }

    /// <summary>
    /// Validates the parameters for STC calculation.
    /// </summary>
    /// <param name="cyclePeriods">The number of periods for the cycle.</param>
    /// <param name="fastPeriods">The number of fast periods for the MACD calculation.</param>
    /// <param name="slowPeriods">The number of slow periods for the MACD calculation.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a parameter is out of range.</exception>
    internal static void Validate(
        int cyclePeriods,
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        if (cyclePeriods < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cyclePeriods), cyclePeriods,
                "Trend Cycle periods must be greater than or equal to 0 for STC.");
        }

        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                "Fast periods must be greater than 0 for STC.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow periods must be greater than the fast period for STC.");
        }
    }
}
