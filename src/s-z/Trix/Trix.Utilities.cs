namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<TrixResult> RemoveWarmupPeriods(
        this IReadOnlyList<TrixResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Trix != null);

        return results.Remove(3 * n + 100);
    }
}
