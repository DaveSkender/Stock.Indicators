namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<SlopeResult> RemoveWarmupPeriods(
        this IReadOnlyList<SlopeResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Slope != null);

        return results.Remove(removePeriods);
    }
}
