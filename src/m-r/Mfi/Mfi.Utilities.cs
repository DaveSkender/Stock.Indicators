namespace Skender.Stock.Indicators;

// MONEY FLOW INDEX (UTILITIES)

public static partial class Mfi
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<MfiResult> RemoveWarmupPeriods(
        this IReadOnlyList<MfiResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Mfi != null);

        return results.Remove(removePeriods);
    }

    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for MFI.");
        }
    }
}
