namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<HurstResult> RemoveWarmupPeriods(
        this IEnumerable<HurstResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.HurstExponent != null);

        return results.Remove(removePeriods);
    }
}
