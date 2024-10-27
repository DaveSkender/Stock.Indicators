namespace Skender.Stock.Indicators;

[Serializable]
public record PvoResult
(
    DateTime Timestamp,
    double? Pvo,
    double? Signal,
    double? Histogram
) : IReusable
{
    public double Value => Pvo.Null2NaN();
}
