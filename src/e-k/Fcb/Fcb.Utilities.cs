namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for Fractal Chaos Bands (FCB) calculations.
/// </summary>
public static partial class Fcb
{
    /// <summary>
    /// Converts FCB results to a chainable list using the specified field.
    /// </summary>
    /// <param name="results">The list of FCB results.</param>
    /// <param name="field">The field to use for chaining.</param>
    /// <returns>A list of chainable values.</returns>
    public static IReadOnlyList<QuotePart> Use(
        this IReadOnlyList<FcbResult> results,
        FcbField field = FcbField.UpperBand)
    {
        ArgumentNullException.ThrowIfNull(results);
        int length = results.Count;
        List<QuotePart> list = new(length);

        for (int i = 0; i < length; i++)
        {
            FcbResult r = results[i];

            double value = field switch {
                FcbField.UpperBand => (double?)r.UpperBand ?? double.NaN,
                FcbField.LowerBand => (double?)r.LowerBand ?? double.NaN,
                _ => throw new ArgumentOutOfRangeException(nameof(field), field, "Invalid field provided.")
            };

            list.Add(new QuotePart(r.Timestamp, value));
        }

        return list;
    }

    /// <summary>
    /// Removes empty (null) periods from the FCB results.
    /// </summary>
    /// <inheritdoc cref="Reusable.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<FcbResult> Condense(
        this IReadOnlyList<FcbResult> results)
    {
        List<FcbResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                static x => x.UpperBand is null && x.LowerBand is null);

        return resultsList.ToSortedList();
    }

    /// <summary>
    /// Removes the recommended warmup periods from the FCB results.
    /// </summary>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<FcbResult> RemoveWarmupPeriods(
        this IReadOnlyList<FcbResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        int removePeriods = results
            .FindIndex(static x => x.UpperBand != null || x.LowerBand != null);

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
