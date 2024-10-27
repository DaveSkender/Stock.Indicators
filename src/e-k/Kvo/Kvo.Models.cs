namespace Skender.Stock.Indicators;

[Serializable]
public record KvoResult
(
    DateTime Timestamp,
    double? Oscillator = null,
    double? Signal = null
) : IReusable
{
    public double Value => Oscillator.Null2NaN();
}
