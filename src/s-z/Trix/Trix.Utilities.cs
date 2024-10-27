namespace Skender.Stock.Indicators;

// TRIPLE EMA OSCILLATOR - TRIX (UTILITIES)

public static partial class Trix
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<TrixResult> RemoveWarmupPeriods(
        this IReadOnlyList<TrixResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Trix != null);

        return results.Remove(3 * n + 100);
    }

    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for TRIX.");
        }
    }
}
