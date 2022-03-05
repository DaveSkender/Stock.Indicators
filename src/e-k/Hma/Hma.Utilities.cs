namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<HmaResult> RemoveWarmupPeriods(
        this IEnumerable<HmaResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Hma != null);

        return results.Remove(removePeriods);
    }
}
