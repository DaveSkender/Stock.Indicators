namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<VwmaResult> RemoveWarmupPeriods(
        this IReadOnlyList<VwmaResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Vwma != null);

        return results.Remove(removePeriods);
    }
}
