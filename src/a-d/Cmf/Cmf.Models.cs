namespace Skender.Stock.Indicators;

public record struct CmfResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? MoneyFlowMultiplier { get; set; }
    public double? MoneyFlowVolume { get; set; }
    public double? Cmf { get; set; }

    readonly double IReusableResult.Value
        => Cmf.Null2NaN();
}
