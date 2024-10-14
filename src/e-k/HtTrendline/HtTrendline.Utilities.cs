namespace Skender.Stock.Indicators;

// HILBERT TRANSFORM - INSTANTANEOUS TRENDLINE (UTILITIES)

public static partial class HtTrendline
{
    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<HtlResult> RemoveWarmupPeriods(
        this IReadOnlyList<HtlResult> results)
            => results.Remove(100);
}
