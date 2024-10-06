namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <inheritdoc cref="Utility.Condense{T}(IEnumerable{T})"/>
    public static IReadOnlyList<ZigZagResult> Condense(
        this IEnumerable<ZigZagResult> results)
    {
        List<ZigZagResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.PointType is null);

        return resultsList.ToSortedList();
    }
}
