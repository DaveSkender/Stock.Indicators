namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<CorrResult> RemoveWarmupPeriods(
        this IEnumerable<CorrResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.Correlation != null);

        return results.Remove(removePeriods);
    }
}
