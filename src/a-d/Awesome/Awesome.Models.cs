namespace Skender.Stock.Indicators;

[Serializable]
public record AwesomeResult
(
    DateTime Timestamp,
    double? Oscillator,
    double? Normalized
) : IReusable
{
    public double Value => Oscillator.Null2NaN();
}
