namespace Skender.Stock.Indicators;

public record HurstResult
(
    DateTime Timestamp,
    double? HurstExponent
) : Reusable(Timestamp)
{
    public override double Value => HurstExponent.Null2NaN();
}
