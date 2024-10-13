namespace Skender.Stock.Indicators;

public record FisherTransformResult
(
    DateTime Timestamp,
    double? Fisher,
    double? Trigger
) : IReusable
{
    public double Value => Fisher.Null2NaN();
}
