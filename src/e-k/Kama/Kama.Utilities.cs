namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<KamaResult> RemoveWarmupPeriods(
        this IEnumerable<KamaResult> results)
    {
        int erPeriods = results
            .ToList()
            .FindIndex(x => x.Er != null);

        return results.Remove(Math.Max(erPeriods + 100, 10 * erPeriods));
    }
}
