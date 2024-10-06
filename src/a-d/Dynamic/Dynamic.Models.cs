namespace Skender.Stock.Indicators;

public record DynamicResult
(
    DateTime Timestamp,
    double? Dynamic
) : Reusable(Timestamp)
{
    public override double Value => Dynamic.Null2NaN();
}
