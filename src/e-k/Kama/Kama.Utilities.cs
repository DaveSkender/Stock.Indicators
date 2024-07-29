namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<KamaResult> RemoveWarmupPeriods(
        this IEnumerable<KamaResult> results)
    {
        int erPeriods = results
            .ToList()
            .FindIndex(x => x.Er != null);

        return results.Remove(Math.Max(erPeriods + 100, 10 * erPeriods));
    }
}
