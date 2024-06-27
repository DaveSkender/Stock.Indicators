namespace Skender.Stock.Indicators;

/// <include file='./info.xml' path='info/type[@name="Results"]/*' />
///
public record struct StochResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Oscillator { get; set; }
    public double? Signal { get; set; }
    public double? PercentJ { get; set; }

    // aliases
    public readonly double? K => Oscillator;
    public readonly double? D => Signal;
    public readonly double? J => PercentJ;

    readonly double IReusable.Value
        => Oscillator.Null2NaN();
}
