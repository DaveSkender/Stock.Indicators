namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<TsiResult> RemoveWarmupPeriods(
        this IEnumerable<TsiResult> results)
    {
        int nm = results
            .ToList()
            .FindIndex(x => x.Tsi != null) + 1;

        return results.Remove(nm + 250);
    }
}
