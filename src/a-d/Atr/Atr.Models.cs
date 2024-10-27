namespace Skender.Stock.Indicators;

[Serializable]
public record AtrResult
(
    DateTime Timestamp,
    double? Tr = null,
    double? Atr = null,
    double? Atrp = null
) : IReusable
{
    public double Value => Atrp.Null2NaN();
}
