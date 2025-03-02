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
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => T3.Null2NaN();
}
