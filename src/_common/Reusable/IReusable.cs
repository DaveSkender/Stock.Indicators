namespace Skender.Stock.Indicators;

/// <summary>
/// A time-series type that identifies
/// a single chainable value.
/// </summary>
public interface IReusable : ISeries
{
    /// <summary>
    /// Value that is passed to chained indicators.
    /// </summary>
    [JsonIgnore]
    double Value { get; }
}

/// <summary>
/// Defines a series that provides a value to be passed to chained indicators.
/// </summary>
public interface IReusableX : ISeries
{
    /// <summary>
    /// Value that is passed to chained indicators.
    /// </summary>
    [JsonIgnore]
    long Value { get; }
}
