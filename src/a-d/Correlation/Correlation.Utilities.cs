namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<CorrResult> RemoveWarmupPeriods(
        this IEnumerable<CorrResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.Correlation != null);

        return results.Remove(removePeriods);
    }
}
