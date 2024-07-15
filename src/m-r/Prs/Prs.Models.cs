namespace Skender.Stock.Indicators;

public record PrsResult
(
    DateTime Timestamp,
    double? Prs,
    double? PrsPercent
) : Reusable(Timestamp)
{
    public override double Value => Prs.Null2NaN();
}
