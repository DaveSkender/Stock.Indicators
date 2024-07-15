namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<BopResult> RemoveWarmupPeriods(
        this IEnumerable<BopResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Bop != null);

        return results.Remove(removePeriods);
    }
}
