namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Condense"]/*' />
    ///
    public static IEnumerable<IchimokuResult> Condense(
        this IEnumerable<IchimokuResult> results)
    {
        List<IchimokuResult> resultsList = results
            .ToList();

        _ = resultsList
            .RemoveAll(match: x =>
                   x.TenkanSen is null
                && x.KijunSen is null
                && x.SenkouSpanA is null
                && x.SenkouSpanB is null
                && x.ChikouSpan is null);

        return resultsList.ToSortedList();
    }
}
