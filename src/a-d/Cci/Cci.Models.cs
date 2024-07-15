namespace Skender.Stock.Indicators;

public record CciResult
(
    DateTime Timestamp,
    double? Cci
) : Reusable(Timestamp)
{
    public override double Value => Cci.Null2NaN();
}
