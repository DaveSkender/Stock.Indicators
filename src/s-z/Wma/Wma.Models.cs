namespace Skender.Stock.Indicators;

public record WmaResult
(
    DateTime Timestamp,
    double? Wma
) : Reusable(Timestamp)
{
    public override double Value => Wma.Null2NaN();
}
