namespace Skender.Stock.Indicators;

public record AwesomeResult
(
    DateTime Timestamp,
    double? Oscillator,
    double? Normalized
) : Reusable(Timestamp)
{
    public override double Value => Oscillator.Null2NaN();
}
