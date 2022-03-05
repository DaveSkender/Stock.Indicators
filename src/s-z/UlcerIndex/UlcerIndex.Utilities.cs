namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<UlcerIndexResult> RemoveWarmupPeriods(
        this IEnumerable<UlcerIndexResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.UI != null);

        return results.Remove(removePeriods);
    }
}
