namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<VwapResult> RemoveWarmupPeriods(
        this IEnumerable<VwapResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Vwap != null);

        return results.Remove(removePeriods);
    }
}
