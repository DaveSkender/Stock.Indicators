namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<KeltnerResult> RemoveWarmupPeriods(
        this IEnumerable<KeltnerResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Width != null) + 1;

        return results.Remove(Math.Max(2 * n, n + 100));
    }
}
