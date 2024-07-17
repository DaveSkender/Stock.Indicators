namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<StochRsiResult> RemoveWarmupPeriods(
        this IEnumerable<StochRsiResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.StochRsi != null) + 2;

        return results.Remove(n + 100);
    }
}
