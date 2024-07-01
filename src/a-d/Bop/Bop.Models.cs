namespace Skender.Stock.Indicators;

public readonly record struct BopResult
(
    DateTime Timestamp,
    double? Bop
) : IReusable
{
    double IReusable.Value => Bop.Null2NaN();
}
