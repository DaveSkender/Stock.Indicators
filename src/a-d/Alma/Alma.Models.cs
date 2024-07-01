namespace Skender.Stock.Indicators;

public readonly record struct AlmaResult
(
    DateTime Timestamp,
    double? Alma
) : IReusable
{
    double IReusable.Value => Alma.Null2NaN();
}
