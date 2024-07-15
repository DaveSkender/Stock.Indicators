namespace Skender.Stock.Indicators;

public record AroonResult
(
    DateTime Timestamp,
    double? AroonUp,
    double? AroonDown,
    double? Oscillator
) : Reusable(Timestamp)
{
    public override double Value => Oscillator.Null2NaN();
}
