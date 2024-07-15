namespace Skender.Stock.Indicators;

public record CmoResult
(
    DateTime Timestamp,
    double? Cmo = null
) : Reusable(Timestamp)
{
    public override double Value => Cmo.Null2NaN();
}
