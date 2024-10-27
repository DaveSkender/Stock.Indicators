namespace Skender.Stock.Indicators;

// DOUBLE EXPONENTIAL MOVING AVERAGE (UTILITIES)

public static partial class Dema
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<DemaResult> RemoveWarmupPeriods(
        this IReadOnlyList<DemaResult> results)
    {
        int n = results
          .ToList()
          .FindIndex(x => x.Dema != null) + 1;

        return results.Remove(2 * n + 100);
    }

    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for DEMA.");
        }
    }
}
