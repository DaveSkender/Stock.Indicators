namespace Skender.Stock.Indicators;

public record PmoResult
(
    DateTime Timestamp,
    double? Pmo,
    double? Signal
) : IReusable
{
    public double Value => Pmo.Null2NaN();
}
