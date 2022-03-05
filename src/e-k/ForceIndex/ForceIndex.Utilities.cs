namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<ForceIndexResult> RemoveWarmupPeriods(
        this IEnumerable<ForceIndexResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.ForceIndex != null);

        return results.Remove(n + 100);
    }
}
