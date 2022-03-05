namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<AdxResult> RemoveWarmupPeriods(
        this IEnumerable<AdxResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Pdi != null);

        return results.Remove((2 * n) + 100);
    }
}
