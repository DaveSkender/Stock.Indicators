namespace Skender.Stock.Indicators;

// PRICE MOMENTUM OSCILLATOR (UTILITIES)

public static partial class Pmo
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<PmoResult> RemoveWarmupPeriods(
        this IReadOnlyList<PmoResult> results)
    {
        int ts = results
            .ToList()
            .FindIndex(x => x.Pmo != null) + 1;

        return results.Remove(ts + 250);
    }

    // parameter validation
    internal static void Validate(
        int timePeriods,
        int smoothPeriods,
        int signalPeriods)
    {
        // check parameter arguments
        if (timePeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(timePeriods), timePeriods,
                "Time periods must be greater than 1 for PMO.");
        }

        if (smoothPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smoothPeriods), smoothPeriods,
                "Smoothing periods must be greater than 0 for PMO.");
        }

        if (signalPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                "Signal periods must be greater than 0 for PMO.");
        }
    }
}
