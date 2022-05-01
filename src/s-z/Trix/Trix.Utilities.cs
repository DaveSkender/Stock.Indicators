namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<TrixResult> RemoveWarmupPeriods(
        this IEnumerable<TrixResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Trix != null);

        return results.Remove((3 * n) + 100);
    }
}
