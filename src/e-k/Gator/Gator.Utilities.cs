namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for Gator Oscillator calculations.
/// </summary>
public static partial class Gator
{
    /// <summary>
    /// Removes empty (null) periods from the Gator Oscillator results.
    /// </summary>
    /// <param name="results">The list of Gator Oscillator results.</param>
    /// <returns>A list of Gator Oscillator results with empty periods removed.</returns>
    /// <inheritdoc cref="Reusable.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<GatorResult> Condense(
        this IReadOnlyList<GatorResult> results)
    {
        List<GatorResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.Upper is null && x.Lower is null);

        return resultsList.ToSortedList();
    }

    /// <summary>
    /// Removes the recommended warmup periods from the Gator Oscillator results.
    /// </summary>
    /// <param name="results">The list of Gator Oscillator results.</param>
    /// <returns>A list of Gator Oscillator results with warmup periods removed.</returns>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<GatorResult> RemoveWarmupPeriods(
        this IReadOnlyList<GatorResult> results) => results.Remove(150);
}
