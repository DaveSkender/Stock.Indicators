namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<WilliamsResult> RemoveWarmupPeriods(
        this IReadOnlyList<WilliamsResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.WilliamsR != null);

        return results.Remove(removePeriods);
    }
}
