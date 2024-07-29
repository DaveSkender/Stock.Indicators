namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<AdxResult> RemoveWarmupPeriods(
        this IEnumerable<AdxResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Pdi != null);

        return results.Remove(2 * n + 100);
    }
}
