namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<ChandelierResult> RemoveWarmupPeriods(
        this IEnumerable<ChandelierResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.ChandelierExit != null);

        return results.Remove(removePeriods);
    }
}
