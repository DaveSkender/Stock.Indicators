namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<UlcerIndexResult> RemoveWarmupPeriods(
        this IEnumerable<UlcerIndexResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.UlcerIndex != null);

        return results.Remove(removePeriods);
    }
}
