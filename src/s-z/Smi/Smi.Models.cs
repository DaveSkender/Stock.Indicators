namespace Skender.Stock.Indicators;

public sealed class SmiResult : ResultBase, IReusableResult
{
    public double? Smi { get; set; }
    public double? Signal { get; set; }

    double IReusableResult.Value => Smi.Null2NaN();
}
