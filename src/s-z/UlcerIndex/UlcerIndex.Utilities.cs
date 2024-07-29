namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<UlcerIndexResult> RemoveWarmupPeriods(
        this IEnumerable<UlcerIndexResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.UlcerIndex != null);

        return results.Remove(removePeriods);
    }
}
