namespace Skender.Stock.Indicators;

public readonly record struct StcResult
(
    DateTime Timestamp,
    double? Stc
) : IReusable
{
    double IReusable.Value => Stc.Null2NaN();
}
