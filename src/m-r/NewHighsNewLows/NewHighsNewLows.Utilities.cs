namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<NewHighsNewLowsResult> RemoveWarmupPeriods(
        this IEnumerable<NewHighsNewLowsResult> results)
    {
        int removePeriods = results
           .ToList()
           .FindIndex(x => x.Net != null);

        return results.Remove(removePeriods);
    }
}
