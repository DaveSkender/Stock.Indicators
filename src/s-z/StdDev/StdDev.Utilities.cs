namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<StdDevResult> RemoveWarmupPeriods(
        this IReadOnlyList<StdDevResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.StdDev != null);

        return results.Remove(removePeriods);
    }
}
