namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<UltimateResult> RemoveWarmupPeriods(
        this IEnumerable<UltimateResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Ultimate != null);

        return results.Remove(removePeriods);
    }
}
