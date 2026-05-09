namespace Skender.Stock.Indicators;

/// <summary>
/// Time-series base interface.
/// </summary>
public interface ISeries
{
    /// <summary>
    /// Gets the date/time of the record.
    /// </summary>
    /// <remarks>
    /// For <see cref="ISeries"/> types, this is the
    /// date/time from the matching aggregate quote period.
    /// For <see cref="IQuote"/> types, this is the
    /// Close/end date and time of the OHLCV aggregate period.
    /// From a practical perspective, Timestamp is the correlation ID.
    /// </remarks>
    DateTime Timestamp { get; }

    // TODO: consider adding (long) UnixDate (seconds) to ISeries

    /// <summary>
    /// Gets the date/time of the record.
    /// </summary>
    /// <remarks>
    /// Deprecated. Use 'Timestamp' instead.
    /// </remarks>
    [Obsolete("Deprecated. Use 'Timestamp' instead.")]
    DateTime Date => Timestamp;
}
