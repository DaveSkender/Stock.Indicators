namespace Skender.Stock.Indicators;

public record AroonResult
(
    DateTime Timestamp,
    double? AroonUp,
    double? AroonDown,
    double? Oscillator
) : IReusable
{
    public double Value => Oscillator.Null2NaN();
}
