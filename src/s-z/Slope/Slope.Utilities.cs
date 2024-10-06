namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<SlopeResult> RemoveWarmupPeriods(
        this IEnumerable<SlopeResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Slope != null);

        return results.Remove(removePeriods);
    }
}
