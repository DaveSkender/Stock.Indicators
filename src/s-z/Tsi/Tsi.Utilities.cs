namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<TsiResult> RemoveWarmupPeriods(
        this IReadOnlyList<TsiResult> results)
    {
        int nm = results
            .ToList()
            .FindIndex(x => x.Tsi != null) + 1;

        return results.Remove(nm + 250);
    }
}
