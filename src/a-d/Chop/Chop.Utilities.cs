namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<ChopResult> RemoveWarmupPeriods(
        this IEnumerable<ChopResult> results)
    {
        int removePeriods = results
           .ToList()
           .FindIndex(x => x.Chop != null);

        return results.Remove(removePeriods);
    }
}
