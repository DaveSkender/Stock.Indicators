using System.Globalization;

namespace Skender.Stock.Indicators;

// PIVOT POINTS (UTILITIES)

public static partial class PivotPoints
{
    private static readonly CultureInfo invariantCulture = CultureInfo.InvariantCulture;

    private static readonly Calendar calendar = invariantCulture.Calendar;

    // Gets the DTFI properties required by GetWeekOfYear.
    private static readonly CalendarWeekRule calendarWeekRule
        = invariantCulture.DateTimeFormat.CalendarWeekRule;

    private static readonly DayOfWeek firstDayOfWeek
        = invariantCulture.DateTimeFormat.FirstDayOfWeek;

    // remove recommended periods
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<PivotPointsResult> RemoveWarmupPeriods(
        this IReadOnlyList<PivotPointsResult> results)
    {
        int removePeriods = results
            .ToList()  // TODO: is there a no-copy way to do this?  Many places.
            .FindIndex(x => x.PP != null);

        return results.Remove(removePeriods);
    }
}
