namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<VwmaResult> RemoveWarmupPeriods(
        this IEnumerable<VwmaResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Vwma != null);

        return results.Remove(removePeriods);
    }
}
