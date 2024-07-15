namespace Skender.Stock.Indicators;

public record BopResult
(
    DateTime Timestamp,
    double? Bop
) : Reusable(Timestamp)
{
    public override double Value => Bop.Null2NaN();
}
