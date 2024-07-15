namespace Skender.Stock.Indicators;

/// <summary>
/// Cacheable time-series result.
/// </summary>
public interface ISeries
{
    // TODO: consider adding (long) UnixDate (seconds) to ISeries
    // where DateTime would be a calculated property.  Long and DateTime are both 8 bytes;
    // however, UnixDate would be more efficient for serialization and deserialization, and indexing.

    /// <summary>
    /// Date/time of record.
    /// </summary>
    /// <remarks>
    /// For <see cref="ISeries"/> types, this is the
    /// date/time from the matching aggregate quote period.
    /// For <see cref="IQuote"/> types, this is the
    /// Close/end date and time of the OHLCV aggregate period.
    /// From a practical perspective, Timestamp is the correlation ID.
    /// </remarks>
    DateTime Timestamp { get; }
}
