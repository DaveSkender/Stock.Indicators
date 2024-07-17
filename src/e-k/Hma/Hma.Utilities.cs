namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<HmaResult> RemoveWarmupPeriods(
        this IEnumerable<HmaResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Hma != null);

        return results.Remove(removePeriods);
    }
}
