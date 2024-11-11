namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for the Percentage Volume Oscillator (PVO).
/// </summary>
public static partial class Pvo
{
    /// <summary>
    /// Removes the recommended warmup periods from the PVO results.
    /// </summary>
    /// <param name="results">The list of PVO results.</param>
    /// <returns>A list of PVO results without the warmup periods.</returns>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<PvoResult> RemoveWarmupPeriods(
        this IReadOnlyList<PvoResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Signal != null) + 2;

        return results.Remove(n + 250);
    }

    /// <summary>
    /// Validates the parameters for PVO calculations.
    /// </summary>
    /// <param name="fastPeriods">The number of periods for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any parameter is out of range.</exception>
    internal static void Validate(
        int fastPeriods,
        int slowPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                "Fast periods must be greater than 0 for PVO.");
        }

        if (signalPeriods < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                "Signal periods must be greater than or equal to 0 for PVO.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow periods must be greater than the fast period for PVO.");
        }
    }
}
