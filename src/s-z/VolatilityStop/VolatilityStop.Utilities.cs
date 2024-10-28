namespace Skender.Stock.Indicators;

// VOLATILITY SYSTEM/STOP (UTILITIES)

public static partial class VolatilityStop
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<VolatilityStopResult> RemoveWarmupPeriods(
        this IReadOnlyList<VolatilityStopResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Sar != null);

        removePeriods = Math.Max(100, removePeriods);

        return results.Remove(removePeriods);
    }

    // parameter validation
    internal static void Validate(
        int lookbackPeriods,
        double multiplier)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for Volatility Stop.");
        }

        if (multiplier <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(multiplier), multiplier,
                "ATR Multiplier must be greater than 0 for Volatility Stop.");
        }
    }
}
