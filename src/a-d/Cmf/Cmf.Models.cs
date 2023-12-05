namespace Skender.Stock.Indicators;

public sealed class CmfResult : ResultBase, IReusableResult
{
    public double? MoneyFlowMultiplier { get; set; }
    public double? MoneyFlowVolume { get; set; }
    public double? Cmf { get; set; }

    double IReusableResult.Value => Cmf.Null2NaN();
}
