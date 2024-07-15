namespace Skender.Stock.Indicators;

public record ChopResult
(
    DateTime Timestamp,
    double? Chop
) : Reusable(Timestamp)
{
    public override double Value => Chop.Null2NaN();
}
