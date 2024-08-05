namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<WilliamsResult> RemoveWarmupPeriods(
        this IEnumerable<WilliamsResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.WilliamsR != null);

        return results.Remove(removePeriods);
    }
}
