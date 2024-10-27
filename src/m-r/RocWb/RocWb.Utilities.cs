namespace Skender.Stock.Indicators;

// RATE OF CHANGE (ROC) WITH BANDS (UTILITIES)

public static partial class RocWb
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<RocWbResult> RemoveWarmupPeriods(
        this IReadOnlyList<RocWbResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.RocEma != null) + 1;

        return results.Remove(n + 100);
    }

    // parameter validation
    internal static void Validate(
        int lookbackPeriods,
        int emaPeriods,
        int stdDevPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for ROC with Bands.");
        }

        if (emaPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(emaPeriods), emaPeriods,
                "EMA periods must be greater than 0 for ROC.");
        }

        if (stdDevPeriods <= 0 || stdDevPeriods > lookbackPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(stdDevPeriods), stdDevPeriods,
                "Standard Deviation periods must be greater than 0 and less than lookback period for ROC with Bands.");
        }
    }
}
