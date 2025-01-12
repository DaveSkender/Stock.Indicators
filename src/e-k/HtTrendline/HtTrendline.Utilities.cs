/// <summary>
/// Provides utility methods for Hilbert Transform Instantaneous Trendline (HTL) calculations.
/// </summary>
public static partial class HtTrendline
{
    /// <summary>
    /// Removes the recommended warmup periods from the HTL results.
    /// </summary>
    /// <param name="results">The list of HTL results.</param>
    /// <returns>A list of HTL results with the warmup periods removed.</returns>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<HtlResult> RemoveWarmupPeriods(
        this IReadOnlyList<HtlResult> results)
            => results.Remove(100);
}
