namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<CciResult> RemoveWarmupPeriods(
        this IReadOnlyList<CciResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Cci != null);

        return results.Remove(removePeriods);
    }
}
