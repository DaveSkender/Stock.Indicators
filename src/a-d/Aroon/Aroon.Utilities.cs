namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<AroonResult> RemoveWarmupPeriods(
        this IReadOnlyList<AroonResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Oscillator != null);

        return results.Remove(removePeriods);
    }
}
