namespace Skender.Stock.Indicators;

public record CmfResult
(
    DateTime Timestamp,
    double? MoneyFlowMultiplier,
    double? MoneyFlowVolume,
    double? Cmf
) : Reusable(Timestamp, Cmf.Null2NaN());
