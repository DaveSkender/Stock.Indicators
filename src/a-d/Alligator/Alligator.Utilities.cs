namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Condense"]/*' />
    ///
    public static IEnumerable<AlligatorResult> Condense(
        this IEnumerable<AlligatorResult> results)
    {
        List<AlligatorResult> resultsList = results
            .ToList();

        _ = resultsList
            .RemoveAll(match:
                x => x.Jaw is null && x.Teeth is null && x.Lips is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<AlligatorResult> RemoveWarmupPeriods(
        this IEnumerable<AlligatorResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.Jaw != null) + 251;

        return results.Remove(removePeriods);
    }
}
