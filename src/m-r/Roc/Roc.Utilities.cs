namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<RocResult> RemoveWarmupPeriods(
        this IEnumerable<RocResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Roc != null);

        return results.Remove(removePeriods);
    }
}
