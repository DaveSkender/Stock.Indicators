namespace Skender.Stock.Indicators;

public readonly record struct DynamicResult
(
    DateTime Timestamp,
    double? Dynamic
) : IReusable
{
    double IReusable.Value => Dynamic.Null2NaN();
}
