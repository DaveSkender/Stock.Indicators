namespace Skender.Stock.Indicators;

// PIVOT POINTS (UTILITIES)
public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<PivotPointsResult> RemoveWarmupPeriods(
        this IEnumerable<PivotPointsResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.PP != null);

        return results.Remove(removePeriods);
    }
}
