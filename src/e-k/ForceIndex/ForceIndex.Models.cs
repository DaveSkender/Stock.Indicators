namespace Skender.Stock.Indicators;

public record ForceIndexResult
(
    DateTime Timestamp,
    double? ForceIndex = null
) : Reusable(Timestamp)
{
    public override double Value => ForceIndex.Null2NaN();
}
