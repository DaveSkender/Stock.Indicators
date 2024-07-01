namespace Skender.Stock.Indicators;

public readonly record struct StdDevResult
(
    DateTime Timestamp,
    double? StdDev,
    double? Mean,
    double? ZScore
) : IReusable
{
    double IReusable.Value => StdDev.Null2NaN();
}
