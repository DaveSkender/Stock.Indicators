namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<HtlResult> RemoveWarmupPeriods(
        this IEnumerable<HtlResult> results) => results.Remove(100);
}
