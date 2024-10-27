namespace Skender.Stock.Indicators;

[Serializable]
public record WmaResult
(
    DateTime Timestamp,
    double? Wma
) : IReusable
{
    public double Value => Wma.Null2NaN();
}
