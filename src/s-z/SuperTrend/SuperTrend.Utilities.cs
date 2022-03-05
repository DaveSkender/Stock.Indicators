namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<SuperTrendResult> RemoveWarmupPeriods(
        this IEnumerable<SuperTrendResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.SuperTrend != null);

        return results.Remove(removePeriods);
    }
}
