namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <inheritdoc cref="Utility.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<PivotsResult> Condense(
        this IReadOnlyList<PivotsResult> results)
    {
        List<PivotsResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.HighPoint is null && x.LowPoint is null);

        return resultsList.ToSortedList();
    }
}
