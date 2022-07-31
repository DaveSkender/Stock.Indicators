namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<RsiResult> RemoveWarmupPeriods(
        this IEnumerable<RsiResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Rsi != null);

        return results.Remove(10 * n);
    }
}
