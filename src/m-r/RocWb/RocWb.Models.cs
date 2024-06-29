namespace Skender.Stock.Indicators;

public readonly record struct RocWbResult
(
    DateTime Timestamp,
    double? Roc,
    double? RocEma,
    double? UpperBand,
    double? LowerBand
) : IReusable
{
    double IReusable.Value => Roc.Null2NaN();
}
