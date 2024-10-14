namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<EpmaResult> RemoveWarmupPeriods(
        this IReadOnlyList<EpmaResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.Epma != null);

        return results.Remove(removePeriods);
    }
}
