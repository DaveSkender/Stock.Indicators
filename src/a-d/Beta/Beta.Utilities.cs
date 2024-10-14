namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<BetaResult> RemoveWarmupPeriods(
        this IReadOnlyList<BetaResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Beta != null);

        return results.Remove(removePeriods);
    }
}
