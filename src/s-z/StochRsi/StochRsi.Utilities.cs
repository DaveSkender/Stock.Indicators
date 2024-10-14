namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<StochRsiResult> RemoveWarmupPeriods(
        this IReadOnlyList<StochRsiResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.StochRsi != null) + 2;

        return results.Remove(n + 100);
    }
}
