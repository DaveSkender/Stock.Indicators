namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Rate of Change with Bands (RocWb) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the RocWb result.</param>
/// <param name="Roc">The rate of change value of the RocWb result.</param>
/// <param name="RocEma">The exponential moving average of the rate of change.</param>
/// <param name="UpperBand">The upper band value of the RocWb result.</param>
/// <param name="LowerBand">The lower band value of the RocWb result.</param>
[Serializable]
public record RocWbResult
(
    DateTime Timestamp,
    double? Roc,
    double? RocEma,
    double? UpperBand,
    double? LowerBand
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Roc.Null2NaN();
}
