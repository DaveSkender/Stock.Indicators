namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<T3Result> RemoveWarmupPeriods(
        this IEnumerable<T3Result> results)
    {
        int n6 = results
            .ToList()
            .FindIndex(x => x.T3 != null);

        return results.Remove(n6 + 250);
    }
}
