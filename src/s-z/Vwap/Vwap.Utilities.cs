namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<VwapResult> RemoveWarmupPeriods(
        this IEnumerable<VwapResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Vwap != null);

        return results.Remove(removePeriods);
    }
}
