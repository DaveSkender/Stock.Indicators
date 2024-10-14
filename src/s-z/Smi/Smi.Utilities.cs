namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<SmiResult> RemoveWarmupPeriods(
        this IReadOnlyList<SmiResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Smi != null);

        return results.Remove(removePeriods + 2 + 100);
    }
}
