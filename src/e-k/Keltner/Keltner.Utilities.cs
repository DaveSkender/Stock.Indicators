namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Condense"]/*' />
    ///
    public static IEnumerable<KeltnerResult> Condense(
        this IEnumerable<KeltnerResult> results)
    {
        List<KeltnerResult> resultsList = results
            .ToList();

        _ = resultsList
            .RemoveAll(match:
                x => x.UpperBand is null && x.LowerBand is null && x.Centerline is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<KeltnerResult> RemoveWarmupPeriods(
        this IEnumerable<KeltnerResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Width != null) + 1;

        return results.Remove(Math.Max(2 * n, n + 100));
    }
}
