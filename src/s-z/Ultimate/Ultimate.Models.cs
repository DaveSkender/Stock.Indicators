namespace Skender.Stock.Indicators;

public readonly record struct UltimateResult
(
    DateTime Timestamp,
    double? Ultimate
) : IReusable
{
    double IReusable.Value => Ultimate.Null2NaN();
}
