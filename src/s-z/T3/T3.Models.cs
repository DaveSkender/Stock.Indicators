namespace Skender.Stock.Indicators;

public readonly record struct T3Result
(
    DateTime Timestamp,
    double? T3
) : IReusable
{
    double IReusable.Value => T3.Null2NaN();
}
