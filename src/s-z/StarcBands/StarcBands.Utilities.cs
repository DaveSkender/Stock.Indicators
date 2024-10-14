namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <inheritdoc cref="Utility.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<StarcBandsResult> Condense(
        this IReadOnlyList<StarcBandsResult> results)
    {
        List<StarcBandsResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.UpperBand is null && x.LowerBand is null && x.Centerline is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<StarcBandsResult> RemoveWarmupPeriods(
        this IReadOnlyList<StarcBandsResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.UpperBand != null || x.LowerBand != null) + 1;

        return results.Remove(n + 150);
    }
}
