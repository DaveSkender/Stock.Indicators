namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<HmaResult> RemoveWarmupPeriods(
        this IReadOnlyList<HmaResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Hma != null);

        return results.Remove(removePeriods);
    }
}
