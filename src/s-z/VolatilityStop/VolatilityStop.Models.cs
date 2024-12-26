namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Volatility Stop indicator calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the data point.</param>
/// <param name="Sar">The stop and reverse (SAR) value at this point.</param>
/// <param name="IsStop">Indicates if the current point is a stop.</param>
/// <param name="UpperBand">The upper band value at this point.</param>
/// <param name="LowerBand">The lower band value at this point.</param>
[Serializable]
public record VolatilityStopResult
(
    DateTime Timestamp,
    double? Sar = null,
    bool? IsStop = null,

    // SAR values as long/short stop bands
    double? UpperBand = null,
    double? LowerBand = null

) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Sar.Null2NaN();
}
