namespace Skender.Stock.Indicators;

public sealed class HurstResult : ResultBase, IReusableResult
{
    public double? HurstExponent { get; set; }

    double IReusableResult.Value => HurstExponent.Null2NaN();
}
