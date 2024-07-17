namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<PvoResult> RemoveWarmupPeriods(
        this IEnumerable<PvoResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Signal != null) + 2;

        return results.Remove(n + 250);
    }
}
