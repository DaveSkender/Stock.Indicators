namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<CmfResult> RemoveWarmupPeriods(
        this IReadOnlyList<CmfResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.Cmf != null);

        return results.Remove(removePeriods);
    }
}
