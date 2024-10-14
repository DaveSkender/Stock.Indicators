namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<CmoResult> RemoveWarmupPeriods(
        this IReadOnlyList<CmoResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Cmo != null);

        return results.Remove(removePeriods);
    }
}
