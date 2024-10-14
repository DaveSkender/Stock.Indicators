namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<AlmaResult> RemoveWarmupPeriods(
        this IReadOnlyList<AlmaResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Alma != null);

        return results.Remove(removePeriods);
    }
}
