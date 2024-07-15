namespace Skender.Stock.Indicators;

public record StcResult
(
    DateTime Timestamp,
    double? Stc
) : Reusable(Timestamp)
{
    public override double Value => Stc.Null2NaN();
}
