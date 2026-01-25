using Skender.Stock.Indicators;

namespace Test.SseServer;

internal static class Utilities
{
    /// <summary>
    /// Convert TimeSpan to PeriodSize for RandomGbm constructor
    /// </summary>
    /// <param name="timeSpan">Time span to convert</param>
    /// <returns>Closest matching PeriodSize</returns>
    internal static PeriodSize ConvertTimeSpanToPeriodSize(this TimeSpan timeSpan)
    {
        double totalMinutes = timeSpan.TotalMinutes;

        return totalMinutes switch {
            < 2 => PeriodSize.OneMinute,
            < 5 => PeriodSize.TwoMinutes,
            < 8 => PeriodSize.FiveMinutes,
            < 20 => PeriodSize.FifteenMinutes,
            < 45 => PeriodSize.ThirtyMinutes,
            < 90 => PeriodSize.OneHour,
            < 180 => PeriodSize.TwoHours,
            < 360 => PeriodSize.FourHours,
            < 1440 => PeriodSize.Day,
            < 10080 => PeriodSize.Week,
            _ => PeriodSize.Month
        };
    }
}
