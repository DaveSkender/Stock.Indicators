namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<StochRsiResult> RemoveWarmupPeriods(
        this IEnumerable<StochRsiResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.StochRsi != null) + 2;

        return results.Remove(n + 100);
    }
}
