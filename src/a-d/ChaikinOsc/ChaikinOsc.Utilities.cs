namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<ChaikinOscResult> RemoveWarmupPeriods(
        this IReadOnlyList<ChaikinOscResult> results)
    {
        int s = results
            .ToList()
            .FindIndex(x => x.Oscillator != null) + 1;

        return results.Remove(s + 100);
    }
}
