namespace Skender.Stock.Indicators;

public readonly record struct CciResult
(
    DateTime Timestamp,
    double? Cci
) : IReusable
{
    double IReusable.Value => Cci.Null2NaN();
}
