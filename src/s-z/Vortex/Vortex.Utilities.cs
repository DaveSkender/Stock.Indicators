namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Condense"]/*' />
    ///
    public static IEnumerable<VortexResult> Condense(
        this IEnumerable<VortexResult> results)
    {
        List<VortexResult> resultsList = results
            .ToList();

        _ = resultsList
            .RemoveAll(match:
                x => x.Pvi is null && x.Nvi is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<VortexResult> RemoveWarmupPeriods(
        this IEnumerable<VortexResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Pvi != null || x.Nvi != null);

        return results.Remove(removePeriods);
    }
}
