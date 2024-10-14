namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<HtlResult> RemoveWarmupPeriods(
        this IReadOnlyList<HtlResult> results) => results.Remove(100);
}
