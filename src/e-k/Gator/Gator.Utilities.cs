namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <inheritdoc cref="Utility.Condense{T}(IEnumerable{T})"/>
    public static IReadOnlyList<GatorResult> Condense(
        this IEnumerable<GatorResult> results)
    {
        List<GatorResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.Upper is null && x.Lower is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<GatorResult> RemoveWarmupPeriods(
        this IEnumerable<GatorResult> results) => results.Remove(150);
}
