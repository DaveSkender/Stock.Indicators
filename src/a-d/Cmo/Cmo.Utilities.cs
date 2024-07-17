namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<CmoResult> RemoveWarmupPeriods(
        this IEnumerable<CmoResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Cmo != null);

        return results.Remove(removePeriods);
    }
}
