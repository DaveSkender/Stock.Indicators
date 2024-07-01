namespace Skender.Stock.Indicators;

public readonly record struct RocResult
(
    DateTime Timestamp,
    double? Momentum,
    double? Roc
) : IReusable
{
    double IReusable.Value => Roc.Null2NaN();
}
