namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Condense"]/*' />
    ///
    public static IEnumerable<FractalResult> Condense(
        this IEnumerable<FractalResult> results)
    {
        List<FractalResult> resultsList = results
            .ToList();

        _ = resultsList
            .RemoveAll(match:
                x => x.FractalBull is null && x.FractalBear is null);

        return resultsList.ToSortedList();
    }
}
