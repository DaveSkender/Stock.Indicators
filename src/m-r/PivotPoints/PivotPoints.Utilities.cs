using System.Globalization;

namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for pivot points calculations.
/// </summary>
public static partial class PivotPoints
{
    private static readonly CultureInfo invariantCulture
        = CultureInfo.InvariantCulture;

    private static readonly Calendar calendar
        = invariantCulture.Calendar;

    private static readonly CalendarWeekRule calendarWeekRule
        = invariantCulture.DateTimeFormat.CalendarWeekRule;

    private static readonly DayOfWeek firstDayOfWeek
        = invariantCulture.DateTimeFormat.FirstDayOfWeek;

    /// <summary>
    /// Converts Pivot Points results to a chainable list using the specified field.
    /// </summary>
    /// <param name="results">The list of Pivot Points results.</param>
    /// <param name="field">The field to use for chaining.</param>
    /// <returns>A list of chainable values.</returns>
    public static IReadOnlyList<QuotePart> Use(
        this IReadOnlyList<PivotPointsResult> results,
        PivotField field = PivotField.PP)
    {
        ArgumentNullException.ThrowIfNull(results);
        int length = results.Count;
        List<QuotePart> list = new(length);

        for (int i = 0; i < length; i++)
        {
            PivotPointsResult r = results[i];

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
    /// Removes the warmup periods from the pivot points results.
    /// </summary>
    /// <param name="results">The list of pivot points results.</param>
    /// <returns>A list of pivot points results without the warmup periods.</returns>
    public static IReadOnlyList<PivotPointsResult> RemoveWarmupPeriods(
        this IReadOnlyList<PivotPointsResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        int removePeriods = results
            .FindIndex(static x => x.PP != null);

        return results.Remove(removePeriods);
    }
}
