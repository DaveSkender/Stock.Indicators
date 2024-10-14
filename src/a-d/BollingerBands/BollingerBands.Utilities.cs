namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<BollingerBandsResult> RemoveWarmupPeriods(
        this IReadOnlyList<BollingerBandsResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Width != null);

        return results.Remove(removePeriods);
    }
}
