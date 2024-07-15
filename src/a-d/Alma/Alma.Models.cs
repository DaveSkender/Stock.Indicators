namespace Skender.Stock.Indicators;

public record AlmaResult
(
    DateTime Timestamp,
    double? Alma
) : Reusable(Timestamp)
{
    public override double Value => Alma.Null2NaN();
}
