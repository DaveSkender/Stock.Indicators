namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<ConnorsRsiResult> RemoveWarmupPeriods(
        this IEnumerable<ConnorsRsiResult> results)
    {
        int n = results
          .ToList()
          .FindIndex(x => x.ConnorsRsi != null);

        return results.Remove(n);
    }
}
