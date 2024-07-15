namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<VolatilityStopResult> RemoveWarmupPeriods(
        this IEnumerable<VolatilityStopResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Sar != null);

        removePeriods = Math.Max(100, removePeriods);

        return results.Remove(removePeriods);
    }
}
