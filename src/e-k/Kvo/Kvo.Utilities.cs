namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<KvoResult> RemoveWarmupPeriods(
        this IReadOnlyList<KvoResult> results)
    {
        int l = results
            .ToList()
            .FindIndex(x => x.Oscillator != null) - 1;

        return results.Remove(l + 150);
    }
}
