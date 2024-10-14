namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<SmmaResult> RemoveWarmupPeriods(
        this IReadOnlyList<SmmaResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Smma != null) + 1;

        return results.Remove(n + 100);
    }
}
