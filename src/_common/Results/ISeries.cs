namespace Skender.Stock.Indicators;

/// <summary>
/// Cacheable time-series result.
/// </summary>
public interface ISeries
{
    // <summary>
    // Unique correlation ID of the record.
    // </summary>
    // <remarks>
    // Inherited from provider in chaining situations.
    // Primarily for internal use for finding correlating.
    // </remarks>
    //short Id { get; }

    // long UnixDate { get; }

    /// <summary>
    /// Date/time of record.
    /// </summary>
    /// <remarks>
    /// For <see cref="IQuote"/> types, this is the
    /// Close/end date and time of the OHLCV aggregate period.
    /// </remarks>
    DateTime Timestamp { get; }
}
