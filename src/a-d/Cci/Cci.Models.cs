namespace Skender.Stock.Indicators;

[Serializable]
public record CciResult
(
    DateTime Timestamp,
    double? Cci
) : IReusable
{
    public double Value => Cci.Null2NaN();
}
