namespace Skender.Stock.Indicators;

public readonly record struct VwmaResult
(
    DateTime Timestamp,
    double? Vwma
) : IReusable
{
    double IReusable.Value => Vwma.Null2NaN();
}
