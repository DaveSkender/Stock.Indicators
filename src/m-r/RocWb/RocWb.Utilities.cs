namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<RocWbResult> RemoveWarmupPeriods(
        this IEnumerable<RocWbResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.RocEma != null) + 1;

        return results.Remove(n + 100);
    }
}
