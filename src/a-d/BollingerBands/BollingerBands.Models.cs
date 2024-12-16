namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of Bollinger Bands calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Sma">The simple moving average.</param>
/// <param name="UpperBand">The upper Bollinger Band.</param>
/// <param name="LowerBand">The lower Bollinger Band.</param>
/// <param name="PercentB">The %B value.</param>
/// <param name="ZScore">The Z-Score value.</param>
/// <param name="Width">The width of the Bollinger Bands.</param>
[Serializable]
public record BollingerBandsResult
(
    DateTime Timestamp,
    double? Sma = null,
    double? UpperBand = null,
    double? LowerBand = null,
    double? PercentB = null,
    double? ZScore = null,
    double? Width = null
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => PercentB.Null2NaN();
}
