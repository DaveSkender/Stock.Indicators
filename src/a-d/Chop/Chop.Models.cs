namespace Skender.Stock.Indicators;

public readonly record struct ChopResult
(
    DateTime Timestamp,
    double? Chop
) : IReusable
{
    double IReusable.Value => Chop.Null2NaN();
}
