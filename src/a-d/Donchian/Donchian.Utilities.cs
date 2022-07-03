namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Condense"]/*' />
    ///
    public static IEnumerable<DonchianResult> Condense(
        this IEnumerable<DonchianResult> results)
    {
        List<DonchianResult> resultsList = results
            .ToList();

        _ = resultsList
            .RemoveAll(match:
                x => x.UpperBand is null && x.LowerBand is null && x.Centerline is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<DonchianResult> RemoveWarmupPeriods(
        this IEnumerable<DonchianResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.Width != null);

        return results.Remove(removePeriods);
    }
}
