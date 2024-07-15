namespace Skender.Stock.Indicators;

public record TsiResult
(
    DateTime Timestamp,
    double? Tsi = null,
    double? Signal = null
) : Reusable(Timestamp)
{
    public override double Value => Tsi.Null2NaN();
}
