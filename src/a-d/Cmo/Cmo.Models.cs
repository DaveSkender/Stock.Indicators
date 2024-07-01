namespace Skender.Stock.Indicators;

public readonly record struct CmoResult
(
    DateTime Timestamp,
    double? Cmo
) : IReusable
{
    double IReusable.Value => Cmo.Null2NaN();
}
