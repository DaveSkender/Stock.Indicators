namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<DemaResult> RemoveWarmupPeriods(
        this IEnumerable<DemaResult> results)
    {
        int n = results
          .ToList()
          .FindIndex(x => x.Dema != null) + 1;

        return results.Remove((2 * n) + 100);
    }
}
