using System.Globalization;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Tests.Indicators")]
[assembly: InternalsVisibleTo("Tests.Performance")]

namespace Skender.Stock.Indicators;

/// <summary>Technical indicators and overlays.  See
/// <see href = "https://daveskender.github.io/Stock.Indicators/guide/">
///  the Guide</see> for more information.</summary>
public static partial class Indicator
{
    private static readonly CultureInfo EnglishCulture = new("en-US", false);
    private static readonly Calendar EnglishCalendar = EnglishCulture.Calendar;

    // Gets the DTFI properties required by GetWeekOfYear.
    private static readonly CalendarWeekRule EnglishCalendarWeekRule
        = EnglishCulture.DateTimeFormat.CalendarWeekRule;

    private static readonly DayOfWeek EnglishFirstDayOfWeek
        = EnglishCulture.DateTimeFormat.FirstDayOfWeek;
}
