namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<TemaResult> RemoveWarmupPeriods(
        this IReadOnlyList<TemaResult> results)
    {
        int n = results
          .ToList()
          .FindIndex(x => x.Tema != null) + 1;

        return results.Remove(3 * n + 100);
    }
}
