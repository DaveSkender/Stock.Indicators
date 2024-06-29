namespace Skender.Stock.Indicators;

public readonly record struct HmaResult
(
    DateTime Timestamp,
    double? Hma
) : IReusable
{
    double IReusable.Value => Hma.Null2NaN();
}
