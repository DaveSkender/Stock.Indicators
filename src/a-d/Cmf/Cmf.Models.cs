namespace Skender.Stock.Indicators;

public readonly record struct CmfResult
(
    DateTime Timestamp,
    double? MoneyFlowMultiplier,
    double? MoneyFlowVolume,
    double? Cmf
) : IReusable
{
    double IReusable.Value => Cmf.Null2NaN();
}
