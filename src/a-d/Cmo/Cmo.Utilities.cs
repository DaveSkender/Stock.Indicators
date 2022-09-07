namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<CmoResult> RemoveWarmupPeriods(
        this IEnumerable<CmoResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Cmo != null);

        return results.Remove(removePeriods);
    }
}
