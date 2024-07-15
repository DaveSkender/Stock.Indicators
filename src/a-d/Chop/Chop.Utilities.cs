namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<ChopResult> RemoveWarmupPeriods(
        this IEnumerable<ChopResult> results)
    {
        int removePeriods = results
           .ToList()
           .FindIndex(x => x.Chop != null);

        return results.Remove(removePeriods);
    }
}
