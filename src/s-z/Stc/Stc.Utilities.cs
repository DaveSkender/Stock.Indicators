namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<StcResult> RemoveWarmupPeriods(
        this IReadOnlyList<StcResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Stc != null);

        return results.Remove(n + 250);
    }
}
