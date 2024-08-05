namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<VolatilityStopResult> RemoveWarmupPeriods(
        this IEnumerable<VolatilityStopResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Sar != null);

        removePeriods = Math.Max(100, removePeriods);

        return results.Remove(removePeriods);
    }
}
