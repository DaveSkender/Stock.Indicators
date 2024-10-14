namespace Skender.Stock.Indicators;

// HURST EXPONENT (UTILITIES)

public static partial class Hurst
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<HurstResult> RemoveWarmupPeriods(
        this IReadOnlyList<HurstResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.HurstExponent != null);

        return results.Remove(removePeriods);
    }

    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods < 20)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be at least 20 for Hurst Exponent.");
        }
    }
}
