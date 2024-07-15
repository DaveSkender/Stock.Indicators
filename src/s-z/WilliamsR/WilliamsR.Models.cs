namespace Skender.Stock.Indicators;

public record WilliamsResult
(
    DateTime Timestamp,
    double? WilliamsR
) : Reusable(Timestamp)
{
    public override double Value => WilliamsR.Null2NaN();
}
