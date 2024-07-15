namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<VwapResult> RemoveWarmupPeriods(
        this IEnumerable<VwapResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Vwap != null);

        return results.Remove(removePeriods);
    }
}
