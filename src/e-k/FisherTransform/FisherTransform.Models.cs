namespace Skender.Stock.Indicators;

public record FisherTransformResult
(
    DateTime Timestamp,
    double? Fisher,
    double? Trigger
) : Reusable(Timestamp)
{
    public override double Value => Fisher.Null2NaN();
}
