namespace Skender.Stock.Indicators;

public record CmfResult
(
    DateTime Timestamp,
    double? MoneyFlowMultiplier,
    double? MoneyFlowVolume,
    double? Cmf
) : Reusable(Timestamp)
{
    public override double Value => Cmf.Null2NaN();
}
