namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<DemaResult> RemoveWarmupPeriods(
        this IEnumerable<DemaResult> results)
    {
        int n = results
          .ToList()
          .FindIndex(x => x.Dema != null) + 1;

        return results.Remove(2 * n + 100);
    }
}
