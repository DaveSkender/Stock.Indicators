namespace Skender.Stock.Indicators;

public record CmfResult
(
    DateTime Timestamp,
    double? MoneyFlowMultiplier,
    double? MoneyFlowVolume,
    double? Cmf
) : IReusable
{
    public double Value => Cmf.Null2NaN();
}
