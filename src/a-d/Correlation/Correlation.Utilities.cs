namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<CorrResult> RemoveWarmupPeriods(
        this IEnumerable<CorrResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.Correlation != null);

        return results.Remove(removePeriods);
    }
}
