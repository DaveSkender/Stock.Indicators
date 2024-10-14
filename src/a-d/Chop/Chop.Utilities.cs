namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<ChopResult> RemoveWarmupPeriods(
        this IReadOnlyList<ChopResult> results)
    {
        int removePeriods = results
           .ToList()
           .FindIndex(x => x.Chop != null);

        return results.Remove(removePeriods);
    }
}
