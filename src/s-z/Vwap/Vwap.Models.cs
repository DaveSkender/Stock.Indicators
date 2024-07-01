namespace Skender.Stock.Indicators;

public readonly record struct VwapResult
(
    DateTime Timestamp,
    double? Vwap
) : IReusable
{
    double IReusable.Value => Vwap.Null2NaN();
}
