namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<CorrResult> RemoveWarmupPeriods(
        this IReadOnlyList<CorrResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.Correlation != null);

        return results.Remove(removePeriods);
    }
}
