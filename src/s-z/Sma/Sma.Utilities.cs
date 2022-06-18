namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<SmaResult> RemoveWarmupPeriods(
        this IEnumerable<SmaResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Sma != null);

        return results.Remove(removePeriods);
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<SmaAnalysis> RemoveWarmupPeriods(
        this IEnumerable<SmaAnalysis> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Sma != null);

        return results.Remove(removePeriods);
    }
}
