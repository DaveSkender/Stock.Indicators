namespace Skender.Stock.Indicators;

public record KvoResult
(
    DateTime Timestamp,
    double? Oscillator = null,
    double? Signal = null
) : Reusable(Timestamp)
{
    public override double Value => Oscillator.Null2NaN();
}
