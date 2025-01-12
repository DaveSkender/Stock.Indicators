using System.Text.Json.Serialization;

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
