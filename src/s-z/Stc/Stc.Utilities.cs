namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<StcResult> RemoveWarmupPeriods(
        this IEnumerable<StcResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Stc != null);

        return results.Remove(n + 250);
    }
}
