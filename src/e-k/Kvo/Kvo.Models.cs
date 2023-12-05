namespace Skender.Stock.Indicators;

public sealed class KvoResult : ResultBase, IReusableResult
{
    public double? Oscillator { get; set; }
    public double? Signal { get; set; }

    double IReusableResult.Value => Oscillator.Null2NaN();
}
