namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<UltimateResult> RemoveWarmupPeriods(
        this IEnumerable<UltimateResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Ultimate != null);

        return results.Remove(removePeriods);
    }
}
