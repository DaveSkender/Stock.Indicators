namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Condense"]/*' />
    ///
    public static IEnumerable<SuperTrendResult> Condense(
        this IEnumerable<SuperTrendResult> results)
    {
        List<SuperTrendResult> resultsList = results
            .ToList();

        _ = resultsList
            .RemoveAll(match:
                x => x.SuperTrend is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<SuperTrendResult> RemoveWarmupPeriods(
        this IEnumerable<SuperTrendResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.SuperTrend != null);

        return results.Remove(removePeriods);
    }
}
