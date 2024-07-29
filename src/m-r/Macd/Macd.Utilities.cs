namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<MacdResult> RemoveWarmupPeriods(
        this IEnumerable<MacdResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Signal != null) + 2;

        return results.Remove(n + 250);
    }
}
