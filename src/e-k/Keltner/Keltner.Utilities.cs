namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <inheritdoc cref="Utility.Condense{T}(IEnumerable{T})"/>
    public static IReadOnlyList<KeltnerResult> Condense(
        this IEnumerable<KeltnerResult> results)
    {
        List<KeltnerResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.UpperBand is null && x.LowerBand is null && x.Centerline is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<KeltnerResult> RemoveWarmupPeriods(
        this IEnumerable<KeltnerResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Width != null) + 1;

        return results.Remove(Math.Max(2 * n, n + 100));
    }
}
