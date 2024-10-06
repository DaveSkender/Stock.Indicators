namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<KvoResult> RemoveWarmupPeriods(
        this IEnumerable<KvoResult> results)
    {
        int l = results
            .ToList()
            .FindIndex(x => x.Oscillator != null) - 1;

        return results.Remove(l + 150);
    }
}
