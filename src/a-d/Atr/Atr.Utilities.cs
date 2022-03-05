namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<AtrResult> RemoveWarmupPeriods(
        this IEnumerable<AtrResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Atr != null);

        return results.Remove(removePeriods);
    }
}
