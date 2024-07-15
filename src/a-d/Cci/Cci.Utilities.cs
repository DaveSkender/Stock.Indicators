namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<CciResult> RemoveWarmupPeriods(
        this IEnumerable<CciResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Cci != null);

        return results.Remove(removePeriods);
    }
}
