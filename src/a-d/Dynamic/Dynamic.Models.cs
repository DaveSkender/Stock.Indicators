namespace Skender.Stock.Indicators;

[Serializable]
public record DynamicResult
(
    DateTime Timestamp,
    double? Dynamic
) : IReusable
{
    public double Value => Dynamic.Null2NaN();
}
