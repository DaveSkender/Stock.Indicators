namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<StcResult> RemoveWarmupPeriods(
        this IEnumerable<StcResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Stc != null);

        return results.Remove(n + 250);
    }
}
