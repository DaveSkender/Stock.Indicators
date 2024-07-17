namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <inheritdoc cref="ReusableUtility.Condense{T}(IEnumerable{T})"/>
    public static IReadOnlyList<DonchianResult> Condense(
        this IEnumerable<DonchianResult> results)
    {
        List<DonchianResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.UpperBand is null && x.LowerBand is null && x.Centerline is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<DonchianResult> RemoveWarmupPeriods(
        this IEnumerable<DonchianResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.Width != null);

        return results.Remove(removePeriods);
    }
}
