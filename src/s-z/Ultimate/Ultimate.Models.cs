namespace Skender.Stock.Indicators;

public record UltimateResult
(
    DateTime Timestamp,
    double? Ultimate
) : Reusable(Timestamp)
{
    public override double Value => Ultimate.Null2NaN();
}
