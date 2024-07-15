namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<ParabolicSarResult> RemoveWarmupPeriods(
        this IEnumerable<ParabolicSarResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Sar != null);

        return results.Remove(removePeriods);
    }
}
