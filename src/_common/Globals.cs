using System.Globalization;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Tests.Indicators")]
[assembly: InternalsVisibleTo("Tests.Performance")]

namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    private static readonly CultureInfo invCulture = CultureInfo.InvariantCulture;
    private static readonly Calendar invCalendar = invCulture.Calendar;

    // Gets the DTFI properties required by GetWeekOfYear.
    private static readonly CalendarWeekRule invCalendarWeekRule
        = invCulture.DateTimeFormat.CalendarWeekRule;

    private static readonly DayOfWeek invFirstDayOfWeek
        = invCulture.DateTimeFormat.FirstDayOfWeek;
}
