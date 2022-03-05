namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<MamaResult> RemoveWarmupPeriods(
        this IEnumerable<MamaResult> results)
    {
        return results.Remove(50);
    }
}
