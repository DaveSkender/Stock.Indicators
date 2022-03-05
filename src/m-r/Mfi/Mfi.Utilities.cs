namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<MfiResult> RemoveWarmupPeriods(
        this IEnumerable<MfiResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Mfi != null);

        return results.Remove(removePeriods);
    }
}
