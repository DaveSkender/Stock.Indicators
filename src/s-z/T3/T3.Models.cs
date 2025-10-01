namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a T3 indicator calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the data point.</param>
/// <param name="T3">The value of the T3 indicator at this point.</param>
[Serializable]
public record T3Result
(
    DateTime Timestamp,
    double? T3
) : IReusable
{
    // internal state (not exposed publicly) to support robust stream recalculations
    [JsonIgnore] internal double E1 { get; init; }
    [JsonIgnore] internal double E2 { get; init; }
    [JsonIgnore] internal double E3 { get; init; }
    [JsonIgnore] internal double E4 { get; init; }
    [JsonIgnore] internal double E5 { get; init; }
    [JsonIgnore] internal double E6 { get; init; }

    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => T3.Null2NaN();
}
