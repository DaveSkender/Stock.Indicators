namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<TemaResult> RemoveWarmupPeriods(
        this IEnumerable<TemaResult> results)
    {
        int n = results
          .ToList()
          .FindIndex(x => x.Tema != null) + 1;

        return results.Remove((3 * n) + 100);
    }
}
