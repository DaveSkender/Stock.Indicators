namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<StochResult> RemoveWarmupPeriods(
        this IEnumerable<StochResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Oscillator != null);

        return results.Remove(removePeriods);
    }
}
