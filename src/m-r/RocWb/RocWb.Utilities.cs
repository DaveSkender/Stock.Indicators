namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<RocWbResult> RemoveWarmupPeriods(
        this IEnumerable<RocWbResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.RocEma != null) + 1;

        return results.Remove(n + 100);
    }
}
