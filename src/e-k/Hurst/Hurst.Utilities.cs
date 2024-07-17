namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<HurstResult> RemoveWarmupPeriods(
        this IEnumerable<HurstResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.HurstExponent != null);

        return results.Remove(removePeriods);
    }
}
