namespace Skender.Stock.Indicators;

public readonly record struct BollingerBandsResult
(
    DateTime Timestamp,
    double? Sma,
    double? UpperBand,
    double? LowerBand,
    double? PercentB,
    double? ZScore,
    double? Width
) : IReusable
{
    double IReusable.Value => PercentB.Null2NaN();
}
