namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<RollingPivotsResult> RemoveWarmupPeriods(
        this IReadOnlyList<RollingPivotsResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.PP != null);

        return results.Remove(removePeriods);
    }
}
