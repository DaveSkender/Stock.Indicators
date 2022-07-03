namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Condense"]/*' />
    ///
    public static IEnumerable<ZigZagResult> Condense(
        this IEnumerable<ZigZagResult> results)
    {
        List<ZigZagResult> resultsList = results
            .ToList();

        _ = resultsList
            .RemoveAll(match:
                x => x.PointType is null);

        return resultsList.ToSortedList();
    }
}
