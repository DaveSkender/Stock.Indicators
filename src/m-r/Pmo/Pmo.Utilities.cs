namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<PmoResult> RemoveWarmupPeriods(
        this IReadOnlyList<PmoResult> results)
    {
        int ts = results
            .ToList()
            .FindIndex(x => x.Pmo != null) + 1;

        return results.Remove(ts + 250);
    }
}
