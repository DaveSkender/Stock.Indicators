namespace Skender.Stock.Indicators;

public readonly record struct HurstResult
(
    DateTime Timestamp,
    double? HurstExponent
) : IReusable
{
    double IReusable.Value => HurstExponent.Null2NaN();
}
