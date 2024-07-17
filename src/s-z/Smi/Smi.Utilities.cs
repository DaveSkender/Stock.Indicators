namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<SmiResult> RemoveWarmupPeriods(
        this IEnumerable<SmiResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Smi != null);

        return results.Remove(removePeriods + 2 + 100);
    }
}
