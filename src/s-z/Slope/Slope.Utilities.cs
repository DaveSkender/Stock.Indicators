namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<SlopeResult> RemoveWarmupPeriods(
        this IEnumerable<SlopeResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Slope != null);

        return results.Remove(removePeriods);
    }
}
