namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <inheritdoc cref="ReusableUtility.Condense{T}(IEnumerable{T})"/>
    public static IReadOnlyList<FcbResult> Condense(
        this IEnumerable<FcbResult> results)
    {
        List<FcbResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.UpperBand is null && x.LowerBand is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<FcbResult> RemoveWarmupPeriods(
        this IEnumerable<FcbResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.UpperBand != null || x.LowerBand != null);

        return results.Remove(removePeriods);
    }
}
