namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<RocResult> RemoveWarmupPeriods(
        this IEnumerable<RocResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Roc != null);

        return results.Remove(removePeriods);
    }
}
