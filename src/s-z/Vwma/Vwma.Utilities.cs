namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<VwmaResult> RemoveWarmupPeriods(
        this IEnumerable<VwmaResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Vwma != null);

        return results.Remove(removePeriods);
    }
}
