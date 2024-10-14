namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<ForceIndexResult> RemoveWarmupPeriods(
        this IReadOnlyList<ForceIndexResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.ForceIndex != null);

        return results.Remove(n + 100);
    }
}
