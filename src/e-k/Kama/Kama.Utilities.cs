namespace Skender.Stock.Indicators;

public static partial class Kama
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<KamaResult> RemoveWarmupPeriods(
        this IEnumerable<KamaResult> results)
    {
        int erPeriods = results
            .ToList()
            .FindIndex(x => x.Er != null);

        return results.Remove(Math.Max(erPeriods + 100, 10 * erPeriods));
    }

    // parameter validation
    internal static void Validate(
        int erPeriods,
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        if (erPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(erPeriods), erPeriods,
                "Efficiency Ratio periods must be greater than 0 for KAMA.");
        }

        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                "Fast EMA periods must be greater than 0 for KAMA.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow EMA periods must be greater than Fast EMA period for KAMA.");
        }
    }
}
