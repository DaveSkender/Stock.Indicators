namespace Skender.Stock.Indicators;

public record BollingerBandsResult
(
    DateTime Timestamp,
    double? Sma = null,
    double? UpperBand = null,
    double? LowerBand = null,
    double? PercentB = null,
    double? ZScore = null,
    double? Width = null
) : Reusable(Timestamp)
{
    public override double Value => PercentB.Null2NaN();
}
