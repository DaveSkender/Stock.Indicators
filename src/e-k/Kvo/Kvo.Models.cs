namespace Skender.Stock.Indicators;

public record KvoResult
(
    DateTime Timestamp,
    double? Oscillator = null,
    double? Signal = null
) : IReusable
{
    public double Value => Oscillator.Null2NaN();
}
