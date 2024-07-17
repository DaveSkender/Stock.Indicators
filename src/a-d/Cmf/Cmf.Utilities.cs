namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<CmfResult> RemoveWarmupPeriods(
        this IEnumerable<CmfResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.Cmf != null);

        return results.Remove(removePeriods);
    }
}
