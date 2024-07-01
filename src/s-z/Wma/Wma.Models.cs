namespace Skender.Stock.Indicators;

public readonly record struct WmaResult
(
    DateTime Timestamp,
    double? Wma
) : IReusable
{
    double IReusable.Value => Wma.Null2NaN();
}
