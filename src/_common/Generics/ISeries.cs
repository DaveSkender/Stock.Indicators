namespace Skender.Stock.Indicators;

/// <summary>
/// Cacheable time-series result.
/// </summary>
public interface ISeries
{
    /// <summary>
    /// Date/time of record.
    /// </summary>
    /// <remarks>
    /// For <see cref="IQuote"/> types, this is the
    /// Close date/time of the candle/bar.
    /// </remarks>
    DateTime Timestamp { get; }
}
