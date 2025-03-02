namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for Fractal Chaos Bands (FCB) calculations.
/// </summary>
public static partial class Fcb
{
    /// <summary>
    /// Removes empty (null) periods from the FCB results.
    /// </summary>
    /// <param name="results">The list of FCB results.</param>
    /// <returns>A list of FCB results with empty periods removed.</returns>
    /// <inheritdoc cref="Reusable.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<FcbResult> Condense(
        this IReadOnlyList<FcbResult> results)
    {
        List<FcbResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.UpperBand is null && x.LowerBand is null);

        return resultsList.ToSortedList();
    }

    /// <summary>
    /// Removes the recommended warmup periods from the FCB results.
    /// </summary>
    /// <param name="results">The list of FCB results.</param>
    /// <returns>A list of FCB results with warmup periods removed.</returns>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<FcbResult> RemoveWarmupPeriods(
        this IReadOnlyList<FcbResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.UpperBand != null || x.LowerBand != null);

        return results.Remove(removePeriods);
    }

    /// <summary>
    /// Validates the window span for FCB calculations.
    /// </summary>
    /// <param name="windowSpan">The window span for the calculation.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the window span is less than 2.
    /// </exception>
    internal static void Validate(
        int windowSpan)
    {
        // check parameter arguments
        if (windowSpan < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(windowSpan), windowSpan,
                "Window span must be at least 2 for FCB.");
        }
    }
}
