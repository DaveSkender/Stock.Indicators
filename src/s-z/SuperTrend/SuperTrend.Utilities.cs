namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <inheritdoc cref="Reusable.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<SuperTrendResult> Condense(
        this IReadOnlyList<SuperTrendResult> results)
    {
        List<SuperTrendResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.SuperTrend is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<SuperTrendResult> RemoveWarmupPeriods(
        this IReadOnlyList<SuperTrendResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.SuperTrend != null);

        return results.Remove(removePeriods);
    }
}
