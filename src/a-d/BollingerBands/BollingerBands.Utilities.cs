namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<BollingerBandsResult> RemoveWarmupPeriods(
        this IEnumerable<BollingerBandsResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Width != null);

        return results.Remove(removePeriods);
    }
}
