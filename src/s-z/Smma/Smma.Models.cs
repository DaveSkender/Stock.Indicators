namespace Skender.Stock.Indicators;

public record SmmaResult
(
    DateTime Timestamp,
    double? Smma = null
) : Reusable(Timestamp)
{
    public override double Value => Smma.Null2NaN();
}
