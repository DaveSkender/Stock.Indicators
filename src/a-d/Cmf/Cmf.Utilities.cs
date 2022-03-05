namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<CmfResult> RemoveWarmupPeriods(
        this IEnumerable<CmfResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.Cmf != null);

        return results.Remove(removePeriods);
    }
}
