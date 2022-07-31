namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Condense"]/*' />
    ///
    public static IEnumerable<GatorResult> Condense(
        this IEnumerable<GatorResult> results)
    {
        List<GatorResult> resultsList = results
            .ToList();

        _ = resultsList
            .RemoveAll(match:
                x => x.Upper is null && x.Lower is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<GatorResult> RemoveWarmupPeriods(
        this IEnumerable<GatorResult> results) => results.Remove(150);
}
