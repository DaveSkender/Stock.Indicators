namespace Skender.Stock.Indicators;

public readonly record struct WilliamsResult
(
    DateTime Timestamp,
    double? WilliamsR
) : IReusable
{
    double IReusable.Value => WilliamsR.Null2NaN();
}
