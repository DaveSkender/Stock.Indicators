namespace Skender.Stock.Indicators;
#nullable disable

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<DemaResult> RemoveWarmupPeriods(
        this IEnumerable<DemaResult> results)
    {
        int n2 = results
          .ToList()
          .FindIndex(x => x.Dema != null) + 2;

        return results.Remove(n2 + 100);
    }
}
