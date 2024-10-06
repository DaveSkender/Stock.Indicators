namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<SmmaResult> RemoveWarmupPeriods(
        this IEnumerable<SmmaResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Smma != null) + 1;

        return results.Remove(n + 100);
    }
}
