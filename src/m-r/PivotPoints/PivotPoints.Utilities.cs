namespace Skender.Stock.Indicators;

// PIVOT POINTS (UTILITIES)
public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<PivotPointsResult> RemoveWarmupPeriods(
        this IEnumerable<PivotPointsResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.PP != null);

        return results.Remove(removePeriods);
    }
}
