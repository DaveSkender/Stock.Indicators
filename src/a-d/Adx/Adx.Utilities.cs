namespace Skender.Stock.Indicators;

// AVERAGE DIRECTIONAL INDEX (UTILITIES)

public static partial class Adx
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<AdxResult> RemoveWarmupPeriods(
        this IReadOnlyList<AdxResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Pdi != null);

        return results.Remove((2 * n) + 100);
    }

    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for ADX.");
        }
    }
}
