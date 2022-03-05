namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<HtlResult> RemoveWarmupPeriods(
        this IEnumerable<HtlResult> results)
    {
        return results.Remove(100);
    }
}
