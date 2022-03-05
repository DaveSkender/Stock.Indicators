namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<VwmaResult> RemoveWarmupPeriods(
        this IEnumerable<VwmaResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Vwma != null);

        return results.Remove(removePeriods);
    }
}
