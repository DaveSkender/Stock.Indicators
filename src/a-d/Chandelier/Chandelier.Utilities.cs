namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<ChandelierResult> RemoveWarmupPeriods(
        this IReadOnlyList<ChandelierResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.ChandelierExit != null);

        return results.Remove(removePeriods);
    }
}
