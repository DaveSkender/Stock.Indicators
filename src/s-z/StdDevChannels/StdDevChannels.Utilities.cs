namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<StdDevChannelsResult> RemoveWarmupPeriods(
        this IEnumerable<StdDevChannelsResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.UpperChannel != null || x.LowerChannel != null);

        return results.Remove(removePeriods);
    }
}
