namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<UltimateResult> RemoveWarmupPeriods(
        this IReadOnlyList<UltimateResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Ultimate != null);

        return results.Remove(removePeriods);
    }
}
