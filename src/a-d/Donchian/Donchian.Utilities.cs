namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <inheritdoc cref="Utility.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<DonchianResult> Condense(
        this IReadOnlyList<DonchianResult> results)
    {
        List<DonchianResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.UpperBand is null && x.LowerBand is null && x.Centerline is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<DonchianResult> RemoveWarmupPeriods(
        this IReadOnlyList<DonchianResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.Width != null);

        return results.Remove(removePeriods);
    }
}
