namespace Skender.Stock.Indicators;

[Serializable]
public record StochResult
(
    DateTime Timestamp,
    double? Oscillator,
    double? Signal,
    double? PercentJ
) : IReusable
{
    public double Value => Oscillator.Null2NaN();

    // aliases
    public double? K => Oscillator;
    public double? D => Signal;
    public double? J => PercentJ;
}
