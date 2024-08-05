namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<TsiResult> RemoveWarmupPeriods(
        this IEnumerable<TsiResult> results)
    {
        int nm = results
            .ToList()
            .FindIndex(x => x.Tsi != null) + 1;

        return results.Remove(nm + 250);
    }
}
