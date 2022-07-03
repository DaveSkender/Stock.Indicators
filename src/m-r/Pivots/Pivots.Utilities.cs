namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Condense"]/*' />
    ///
    public static IEnumerable<PivotsResult> Condense(
        this IEnumerable<PivotsResult> results)
    {
        List<PivotsResult> resultsList = results
            .ToList();

        _ = resultsList
            .RemoveAll(match:
                x => x.HighPoint is null && x.LowPoint is null);

        return resultsList.ToSortedList();
    }
}
