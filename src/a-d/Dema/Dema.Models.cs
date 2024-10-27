namespace Skender.Stock.Indicators;

[Serializable]
public record DemaResult
(
    DateTime Timestamp,
    double? Dema = null
) : IReusable
{
    public double Value => Dema.Null2NaN();
}
