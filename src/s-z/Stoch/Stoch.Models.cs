namespace Skender.Stock.Indicators;

/// <include file='./info.xml' path='info/type[@name="Results"]/*' />
///
public readonly record struct StochResult
(
    DateTime Timestamp,
    double? Oscillator,
    double? Signal,
    double? PercentJ
) : IReusable
{
    // aliases
    public double? K => Oscillator;
    public double? D => Signal;
    public double? J => PercentJ;

    double IReusable.Value => Oscillator.Null2NaN();
}
