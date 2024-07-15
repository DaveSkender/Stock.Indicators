namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<StdDevResult> RemoveWarmupPeriods(
        this IEnumerable<StdDevResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.StdDev != null);

        return results.Remove(removePeriods);
    }
}
