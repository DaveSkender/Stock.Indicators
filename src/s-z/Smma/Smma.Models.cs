namespace Skender.Stock.Indicators;

[Serializable]
public record SmmaResult
(
    DateTime Timestamp,
    double? Smma = null
) : IReusable
{
    public double Value => Smma.Null2NaN();
}
