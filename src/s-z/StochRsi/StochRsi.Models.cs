namespace Skender.Stock.Indicators;

public record StochRsiResult
(
    DateTime Timestamp,
    double? StochRsi = null,
    double? Signal = null
) : Reusable(Timestamp)
{
    public override double Value => StochRsi.Null2NaN();
}
