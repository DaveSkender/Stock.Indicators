using System;
using System.Globalization;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests.Indicators")]
[assembly: InternalsVisibleTo("Tests.Performance")]
namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        private static readonly CultureInfo englishCulture = new CultureInfo("en-US", false);
        private static readonly Calendar englishCalendar = englishCulture.Calendar;

        // Gets the DTFI properties required by GetWeekOfYear.
        private static readonly CalendarWeekRule englishCalendarWeekRule = englishCulture.DateTimeFormat.CalendarWeekRule;
        private static readonly DayOfWeek englishFirstDayOfWeek = englishCulture.DateTimeFormat.FirstDayOfWeek;
    }
}
