namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <inheritdoc cref="ReusableUtility.Condense{T}(IEnumerable{T})"/>
    public static IEnumerable<SuperTrendResult> Condense(
        this IEnumerable<SuperTrendResult> results)
    {
        List<SuperTrendResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.SuperTrend is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<SuperTrendResult> RemoveWarmupPeriods(
        this IEnumerable<SuperTrendResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.SuperTrend != null);

        return results.Remove(removePeriods);
    }
}
