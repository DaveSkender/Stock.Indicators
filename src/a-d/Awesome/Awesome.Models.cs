namespace Skender.Stock.Indicators;

public record AwesomeResult
(
    DateTime Timestamp,
    double? Oscillator,
    double? Normalized
) : IReusable
{
    public double Value => Oscillator.Null2NaN();
}
