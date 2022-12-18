namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Condense"]/*' />
    ///
    public static IEnumerable<FcbResult> Condense(
        this IEnumerable<FcbResult> results)
    {
        List<FcbResult> resultsList = results
            .ToList();

        _ = resultsList
            .RemoveAll(match:
                x => x.UpperBand is null && x.LowerBand is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<FcbResult> RemoveWarmupPeriods(
        this IEnumerable<FcbResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.UpperBand != null || x.LowerBand != null);

        return results.Remove(removePeriods);
    }
}
