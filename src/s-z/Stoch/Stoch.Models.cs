namespace Skender.Stock.Indicators;

/// <include file='./info.xml' path='info/type[@name="Results"]/*' />
///
[Serializable]
public sealed class StochResult : ResultBase, IReusableResult
{
    public StochResult(DateTime date)
    {
        Date = date;
    }

    public double? Oscillator { get; set; }
    public double? Signal { get; set; }
    public double? PercentJ { get; set; }

    // aliases
    public double? K => Oscillator;
    public double? D => Signal;
    public double? J => PercentJ;

    double? IReusableResult.Value => Oscillator;
}
