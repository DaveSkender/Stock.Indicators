namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<RocResult> RemoveWarmupPeriods(
        this IReadOnlyList<RocResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Roc != null);

        return results.Remove(removePeriods);
    }
}
