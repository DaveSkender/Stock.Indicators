namespace Skender.Stock.Indicators;

public record PmoResult
(
    DateTime Timestamp,
    double? Pmo,
    double? Signal
) : Reusable(Timestamp)
{
    public override double Value => Pmo.Null2NaN();
}
