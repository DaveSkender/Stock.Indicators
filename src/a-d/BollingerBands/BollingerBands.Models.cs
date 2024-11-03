namespace Skender.Stock.Indicators;

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
    public double Value => PercentB.Null2NaN();
}
