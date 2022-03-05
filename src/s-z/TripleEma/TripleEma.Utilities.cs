namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<TemaResult> RemoveWarmupPeriods(
        this IEnumerable<TemaResult> results)
    {
        int n3 = results
          .ToList()
          .FindIndex(x => x.Tema != null) + 3;

        return results.Remove(n3 + 100);
    }
}
