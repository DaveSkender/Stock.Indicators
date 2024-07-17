namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <inheritdoc cref="ReusableUtility.Condense{T}(IEnumerable{T})"/>
    public static IReadOnlyList<PivotsResult> Condense(
        this IEnumerable<PivotsResult> results)
    {
        List<PivotsResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.HighPoint is null && x.LowPoint is null);

        return resultsList.ToSortedList();
    }
}
