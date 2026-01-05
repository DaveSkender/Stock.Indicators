namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for Rolling Pivot Points calculations.
/// </summary>
public static partial class RollingPivots
{
    /// <summary>
    /// Converts Rolling Pivot Points results to a chainable list using the specified field.
    /// </summary>
    /// <param name="results">The list of Rolling Pivot Points results.</param>
    /// <param name="field">The field to use for chaining.</param>
    /// <returns>A list of chainable values.</returns>
    public static IReadOnlyList<QuotePart> Use(
        this IReadOnlyList<RollingPivotsResult> results,
        PivotField field = PivotField.PP)
    {
        ArgumentNullException.ThrowIfNull(results);
        int length = results.Count;
        List<QuotePart> list = new(length);

        for (int i = 0; i < length; i++)
        {
            RollingPivotsResult r = results[i];

            double value = field switch {
                PivotField.PP => (double?)r.PP ?? double.NaN,
                PivotField.S1 => (double?)r.S1 ?? double.NaN,
                PivotField.S2 => (double?)r.S2 ?? double.NaN,
                PivotField.S3 => (double?)r.S3 ?? double.NaN,
                PivotField.S4 => (double?)r.S4 ?? double.NaN,
                PivotField.R1 => (double?)r.R1 ?? double.NaN,
                PivotField.R2 => (double?)r.R2 ?? double.NaN,
                PivotField.R3 => (double?)r.R3 ?? double.NaN,
                PivotField.R4 => (double?)r.R4 ?? double.NaN,
                _ => throw new ArgumentOutOfRangeException(nameof(field), field, "Invalid field provided.")
            };

            list.Add(new QuotePart(r.Timestamp, value));
        }

        return list;
    }

    /// <summary>
    /// Removes the recommended warmup periods from the Rolling Pivots results.
    /// </summary>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<RollingPivotsResult> RemoveWarmupPeriods(
        this IReadOnlyList<RollingPivotsResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        int removePeriods = results
            .FindIndex(static x => x.PP != null);

        return results.Remove(removePeriods);
    }

    /// <summary>
    /// Validates the parameters for Rolling Pivots calculations.
    /// </summary>
    /// <param name="windowPeriods">The number of periods in the rolling window.</param>
    /// <param name="offsetPeriods">The number of periods to offset the window.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are out of range.</exception>
    internal static void Validate(
        int windowPeriods,
        int offsetPeriods)
    {
        // check parameter arguments
        if (windowPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(windowPeriods), windowPeriods,
                "Window periods must be greater than 0 for Rolling Pivot Points.");
        }

        if (offsetPeriods < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offsetPeriods), offsetPeriods,
                "Offset periods must be greater than or equal to 0 for Rolling Pivot Points.");
        }
    }
}
