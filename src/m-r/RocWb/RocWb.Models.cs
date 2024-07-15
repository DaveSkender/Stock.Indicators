namespace Skender.Stock.Indicators;

public record RocWbResult
(
    DateTime Timestamp,
    double? Roc,
    double? RocEma,
    double? UpperBand,
    double? LowerBand
) : Reusable(Timestamp)
{
    public override double Value => Roc.Null2NaN();
}
