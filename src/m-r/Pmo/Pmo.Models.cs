namespace Skender.Stock.Indicators;

[Serializable]
public record PmoResult
(
    DateTime Timestamp,
    double? Pmo,
    double? Signal
) : IReusable
{
    public double Value => Pmo.Null2NaN();
}
