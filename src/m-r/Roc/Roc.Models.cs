namespace Skender.Stock.Indicators;

public record RocResult
(
    DateTime Timestamp,
    double? Momentum,
    double? Roc
) : Reusable(Timestamp)
{
    public override double Value => Roc.Null2NaN();
}
