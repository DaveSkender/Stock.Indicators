namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<SmaExtendedResult> RemoveWarmupPeriods(
        this IEnumerable<SmaExtendedResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Sma != null);

        return results.Remove(removePeriods);
    }
}
