namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<BetaResult> RemoveWarmupPeriods(
        this IEnumerable<BetaResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Beta != null);

        return results.Remove(removePeriods);
    }
}
