namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<RsiResult> RemoveWarmupPeriods(
        this IEnumerable<RsiResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Rsi != null);

        return results.Remove(10 * n);
    }
}
