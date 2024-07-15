namespace Skender.Stock.Indicators;

public record StdDevResult
(
    DateTime Timestamp,
    double? StdDev,
    double? Mean,
    double? ZScore
) : Reusable(Timestamp)
{
    public override double Value => StdDev.Null2NaN();
}
