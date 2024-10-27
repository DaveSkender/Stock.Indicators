namespace Skender.Stock.Indicators;

[Serializable]
public record HurstResult
(
    DateTime Timestamp,
    double? HurstExponent
) : IReusable
{
    public double Value => HurstExponent.Null2NaN();
}
