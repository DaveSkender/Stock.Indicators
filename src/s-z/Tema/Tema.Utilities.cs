namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<TemaResult> RemoveWarmupPeriods(
        this IEnumerable<TemaResult> results)
    {
        int n = results
          .ToList()
          .FindIndex(x => x.Tema != null) + 1;

        return results.Remove(3 * n + 100);
    }
}
