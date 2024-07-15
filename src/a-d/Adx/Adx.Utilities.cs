namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<AdxResult> RemoveWarmupPeriods(
        this IEnumerable<AdxResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Pdi != null);

        return results.Remove(2 * n + 100);
    }
}
