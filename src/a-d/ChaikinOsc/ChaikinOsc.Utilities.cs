namespace Skender.Stock.Indicators;

// CHAIKIN OSCILLATOR (UTILITIES)

public static partial class ChaikinOsc
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<ChaikinOscResult> RemoveWarmupPeriods(
        this IReadOnlyList<ChaikinOscResult> results)
    {
        int s = results
            .ToList()
            .FindIndex(x => x.Oscillator != null) + 1;

        return results.Remove(s + 100);
    }

    // parameter validation
    internal static void Validate(
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                "Fast lookback periods must be greater than 0 for Chaikin Oscillator.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow lookback periods must be greater than Fast lookback period for Chaikin Oscillator.");
        }
    }
}
