namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<WmaResult> RemoveWarmupPeriods(
        this IEnumerable<WmaResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Wma != null);

        return results.Remove(removePeriods);
    }
}
