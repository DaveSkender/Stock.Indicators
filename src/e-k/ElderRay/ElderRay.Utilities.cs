namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<ElderRayResult> RemoveWarmupPeriods(
        this IEnumerable<ElderRayResult> results)
    {
        int n = results
          .ToList()
          .FindIndex(x => x.BullPower != null) + 1;

        return results.Remove(n + 100);
    }
}
