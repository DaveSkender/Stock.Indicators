namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<DemaResult> RemoveWarmupPeriods(
        this IEnumerable<DemaResult> results)
    {
        int n = results
          .ToList()
          .FindIndex(x => x.Dema != null) + 1;

        return results.Remove(2 * n + 100);
    }
}
