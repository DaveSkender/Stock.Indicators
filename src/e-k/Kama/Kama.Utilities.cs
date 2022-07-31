namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<KamaResult> RemoveWarmupPeriods(
        this IEnumerable<KamaResult> results)
    {
        int erPeriods = results
            .ToList()
            .FindIndex(x => x.ER != null);

        return results.Remove(Math.Max(erPeriods + 100, 10 * erPeriods));
    }
}
