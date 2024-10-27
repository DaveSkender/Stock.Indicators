namespace Skender.Stock.Indicators;

[Serializable]
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
