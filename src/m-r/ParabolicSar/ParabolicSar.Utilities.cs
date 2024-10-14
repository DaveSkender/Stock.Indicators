namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<ParabolicSarResult> RemoveWarmupPeriods(
        this IReadOnlyList<ParabolicSarResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Sar != null);

        return results.Remove(removePeriods);
    }
}
