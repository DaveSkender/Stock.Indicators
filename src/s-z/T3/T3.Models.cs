namespace Skender.Stock.Indicators;

[Serializable]
public record T3Result
(
    DateTime Timestamp,
    double? T3
) : IReusable
{
    public double Value => T3.Null2NaN();
}
