namespace Skender.Stock.Indicators;

public sealed record class CmfResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? MoneyFlowMultiplier { get; set; }
    public double? MoneyFlowVolume { get; set; }
    public double? Cmf { get; set; }

    double IReusableResult.Value => Cmf.Null2NaN();
}
