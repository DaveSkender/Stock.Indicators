namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<EpmaResult> RemoveWarmupPeriods(
        this IEnumerable<EpmaResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.Epma != null);

        return results.Remove(removePeriods);
    }
}
