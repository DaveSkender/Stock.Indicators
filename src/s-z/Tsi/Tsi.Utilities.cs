/// <summary>
/// Provides utility methods for calculating the True Strength Index (TSI).
/// </summary>
public static partial class Tsi
{
    // remove recommended periods
    /// <summary>
    /// Removes the warmup periods from the TSI results.
    /// </summary>
    /// <param name="results">The TSI results.</param>
    /// <returns>The TSI results without the warmup periods.</returns>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<TsiResult> RemoveWarmupPeriods(
        this IReadOnlyList<TsiResult> results)
    {
        int nm = results
            .ToList()
            .FindIndex(x => x.Tsi != null) + 1;

        return results.Remove(nm + 250);
    }

    // parameter validation
    /// <summary>
    /// Validates the parameters for the TSI calculation.
    /// </summary>
    /// <param name="lookbackPeriods">The lookback periods.</param>
    /// <param name="smoothPeriods">The smoothing periods.</param>
    /// <param name="signalPeriods">The signal periods.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are out of range.</exception>
    internal static void Validate(
        int lookbackPeriods,
        int smoothPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for TSI.");
        }

        if (smoothPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smoothPeriods), smoothPeriods,
                "Smoothing periods must be greater than 0 for TSI.");
        }

        if (signalPeriods < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                "Signal periods must be greater than or equal to 0 for TSI.");
        }
    }
}
