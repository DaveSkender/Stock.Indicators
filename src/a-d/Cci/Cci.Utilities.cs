namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<CciResult> RemoveWarmupPeriods(
        this IEnumerable<CciResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Cci != null);

        return results.Remove(removePeriods);
    }
}
