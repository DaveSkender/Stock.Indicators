namespace Skender.Stock.Indicators;

public readonly record struct PrsResult
(
    DateTime Timestamp,
    double? Prs,
    double? PrsPercent
) : IReusable
{
    double IReusable.Value => Prs.Null2NaN();
}
