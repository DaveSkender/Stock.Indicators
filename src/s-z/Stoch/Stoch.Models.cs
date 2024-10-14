namespace Skender.Stock.Indicators;

/// <include file='./info.xml' path='info/type[@name="Results"]/*' />
///
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
