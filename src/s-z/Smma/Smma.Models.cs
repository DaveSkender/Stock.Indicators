namespace Skender.Stock.Indicators;

public readonly record struct SmmaResult
(
    DateTime Timestamp,
    double? Smma
) : IReusable
{
    double IReusable.Value => Smma.Null2NaN();
}
