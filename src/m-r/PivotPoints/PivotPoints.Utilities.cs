namespace Skender.Stock.Indicators;

// PIVOT POINTS (UTILITIES)
public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<PivotPointsResult> RemoveWarmupPeriods(
        this IEnumerable<PivotPointsResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.PP != null);

        return results.Remove(removePeriods);
    }
}
