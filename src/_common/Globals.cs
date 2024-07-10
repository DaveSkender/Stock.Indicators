using System.Globalization;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Tests.Indicators")]
[assembly: InternalsVisibleTo("Tests.Performance")]

namespace Skender.Stock.Indicators;

/// <summary>Technical indicators and overlays.  See
/// <see href = "https://dotnet.stockindicators.dev/guide/">
///  the Guide</see> for more information.</summary>
public static partial class Indicator
{
    // Culture info for error messages only (important)

    private const string culture = "en-US";

    private static readonly Calendar EnglishCalendar
        = new CultureInfo(culture, false).Calendar;

    // Gets the DTFI properties required by GetWeekOfYear.
    private static readonly CalendarWeekRule EnglishCalendarWeekRule
        = new CultureInfo(culture, false).DateTimeFormat.CalendarWeekRule;

    private static readonly DayOfWeek EnglishFirstDayOfWeek
        = new CultureInfo(culture, false).DateTimeFormat.FirstDayOfWeek;
}
