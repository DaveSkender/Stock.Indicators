namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Condense"]/*' />
    ///
    public static IEnumerable<AtrStopResult> Condense(
        this IEnumerable<AtrStopResult> results)
    {
        List<AtrStopResult> resultsList = results
            .ToList();

        resultsList.RemoveAll(match: static x => x.AtrStop is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<AtrStopResult> RemoveWarmupPeriods(
        this IEnumerable<AtrStopResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(static x => x.AtrStop != null);

        return results.Remove(removePeriods);
    }
}
