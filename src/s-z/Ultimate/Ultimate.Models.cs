namespace Skender.Stock.Indicators;

public record UltimateResult
(
    DateTime Timestamp,
    double? Ultimate
) : IReusable
{
    public double Value => Ultimate.Null2NaN();
}
