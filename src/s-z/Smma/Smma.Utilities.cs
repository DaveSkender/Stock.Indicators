namespace Skender.Stock.Indicators;

public static partial class Smma
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<SmmaResult> RemoveWarmupPeriods(
        this IReadOnlyList<SmmaResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Smma != null) + 1;

        return results.Remove(n + 100);
    }

    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for SMMA.");
        }
    }
}
