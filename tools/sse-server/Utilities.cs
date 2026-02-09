using Skender.Stock.Indicators;

namespace Test.SseServer;

internal static class Utilities
{
    /// <summary>
    /// Parse quote interval string directly to PeriodSize for RandomGbm constructor.
    /// Supports formats: "1m", "2m", "5m", "15m", "30m", "1h", "2h", "4h", "1d"/"1day", "1w"/"1week".
    /// </summary>
    /// <param name="intervalString">Interval string (e.g., "1h", "5m", "1day")</param>
    /// <returns>Matching PeriodSize</returns>
    /// <exception cref="ArgumentException">Thrown when interval format is invalid or unsupported</exception>
    internal static PeriodSize ParseQuoteIntervalToPeriodSize(string intervalString)
    {
        if (string.IsNullOrWhiteSpace(intervalString))
        {
            throw new ArgumentException("Interval string cannot be null or empty", nameof(intervalString));
        }

        string normalized = intervalString.Trim().ToLowerInvariant();

        return normalized switch {
            "1m" or "1min" or "1minute" => PeriodSize.OneMinute,
            "2m" or "2min" or "2minutes" => PeriodSize.TwoMinutes,
            "5m" or "5min" or "5minutes" => PeriodSize.FiveMinutes,
            "15m" or "15min" or "15minutes" => PeriodSize.FifteenMinutes,
            "30m" or "30min" or "30minutes" => PeriodSize.ThirtyMinutes,
            "1h" or "1hr" or "1hour" => PeriodSize.OneHour,
            "2h" or "2hr" or "2hours" => PeriodSize.TwoHours,
            "4h" or "4hr" or "4hours" => PeriodSize.FourHours,
            "1d" or "1day" or "day" => PeriodSize.Day,
            "1w" or "1week" or "week" => PeriodSize.Week,
            _ => throw new ArgumentException(
                $"Unsupported interval '{intervalString}'. Valid values: 1m, 2m, 5m, 15m, 30m, 1h, 2h, 4h, 1d, 1w",
                nameof(intervalString))
        };
    }
}

internal sealed record SseQuoteAction(string EventType, QuoteAction Payload)
{
    public static SseQuoteAction Add(Quote quote)
        => new("add", new QuoteAction(quote, null));

    public static SseQuoteAction Remove(int cacheIndex, Quote quote)
        => new("remove", new QuoteAction(quote, cacheIndex));
}

internal sealed record QuoteAction(Quote Quote, int? CacheIndex);
