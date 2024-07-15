namespace Skender.Stock.Indicators;

public record AtrResult
(
    DateTime Timestamp,
    double? Tr = null,
    double? Atr = null,
    double? Atrp = null
) : Reusable(Timestamp)
{
    public override double Value => Atrp.Null2NaN();
}
