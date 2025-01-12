/// <summary>
/// Provides utility methods for Relative Strength Index (RSI) calculations.
/// </summary>
public static partial class Rsi
{
    /// <summary>
    /// Removes the recommended warmup periods from the RSI results.
    /// </summary>
    /// <param name="results">The list of RSI results.</param>
    /// <returns>A list of RSI results with the warmup periods removed.</returns>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<RsiResult> RemoveWarmupPeriods(
        this IReadOnlyList<RsiResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Rsi != null);

        return results.Remove(10 * n);
    }

    /// <summary>
    /// Validates the parameters for RSI calculations.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the RSI calculation.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are less than 1.</exception>
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for RSI.");
        }
    }
}
