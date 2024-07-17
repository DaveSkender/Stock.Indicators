namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <inheritdoc cref="ReusableUtility.Condense{T}(IEnumerable{T})"/>
    public static IReadOnlyList<FractalResult> Condense(
        this IEnumerable<FractalResult> results)
    {
        List<FractalResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.FractalBull is null && x.FractalBear is null);

        return resultsList.ToSortedList();
    }
}
