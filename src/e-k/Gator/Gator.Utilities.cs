namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<GatorResult> RemoveWarmupPeriods(
        this IEnumerable<GatorResult> results)
    {
        return results.Remove(150);
    }
}
