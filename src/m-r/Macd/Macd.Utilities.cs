namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<MacdResult> RemoveWarmupPeriods(
        this IEnumerable<MacdResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Signal != null) + 2;

        return results.Remove(n + 250);
    }
}
