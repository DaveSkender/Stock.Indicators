namespace Skender.Stock.Indicators;

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
