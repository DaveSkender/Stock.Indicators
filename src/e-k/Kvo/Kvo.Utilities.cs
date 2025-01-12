// KLINGER VOLUME OSCILLATOR (UTILIITES)

public static partial class Kvo
{
    /// <summary>
    /// Removes the recommended warmup periods from the KVO (Klinger Volume Oscillator) results.
    /// </summary>
    /// <param name="results">The list of KVO results to process.</param>
    /// <returns>A list of KVO results with the warmup periods removed.</returns>
    public static IReadOnlyList<KvoResult> RemoveWarmupPeriods(
        this IReadOnlyList<KvoResult> results)
    {
        int l = results
            .ToList()
            .FindIndex(x => x.Oscillator != null) - 1;

        return results.Remove(l + 150);
    }

    /// <summary>
    /// Validates the parameters for the KVO (Klinger Volume Oscillator) calculation.
    /// </summary>
    /// <param name="fastPeriods">The number of periods for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when any of the parameters are out of their valid range.
    /// </exception>
    internal static void Validate(
        int fastPeriods,
        int slowPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        if (fastPeriods <= 2)
        {
            throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                "Fast (short) Periods must be greater than 2 for Klinger Oscillator.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow (long) Periods must be greater than Fast Periods for Klinger Oscillator.");
        }

        if (signalPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                "Signal Periods must be greater than 0 for Klinger Oscillator.");
        }
    }
}
