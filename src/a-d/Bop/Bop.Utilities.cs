namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<BopResult> RemoveWarmupPeriods(
        this IReadOnlyList<BopResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Bop != null);

        return results.Remove(removePeriods);
    }
}
