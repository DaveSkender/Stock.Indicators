namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<PvoResult> RemoveWarmupPeriods(
        this IReadOnlyList<PvoResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Signal != null) + 2;

        return results.Remove(n + 250);
    }
}
