namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<PmoResult> RemoveWarmupPeriods(
        this IEnumerable<PmoResult> results)
    {
        int ts = results
            .ToList()
            .FindIndex(x => x.Pmo != null) + 1;

        return results.Remove(ts + 250);
    }
}
