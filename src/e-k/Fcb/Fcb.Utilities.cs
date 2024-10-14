namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <inheritdoc cref="Utility.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<FcbResult> Condense(
        this IReadOnlyList<FcbResult> results)
    {
        List<FcbResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.UpperBand is null && x.LowerBand is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<FcbResult> RemoveWarmupPeriods(
        this IReadOnlyList<FcbResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.UpperBand != null || x.LowerBand != null);

        return results.Remove(removePeriods);
    }
}
