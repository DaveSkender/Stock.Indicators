namespace Skender.Stock.Indicators;

[Serializable]
public record CmoResult
(
    DateTime Timestamp,
    double? Cmo = null
) : IReusable
{
    public double Value => Cmo.Null2NaN();
}
