namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<StarcBandsResult> RemoveWarmupPeriods(
        this IEnumerable<StarcBandsResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.UpperBand != null || x.LowerBand != null) + 1;

        return results.Remove(n + 150);
    }
}
