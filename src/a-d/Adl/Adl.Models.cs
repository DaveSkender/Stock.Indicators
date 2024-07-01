namespace Skender.Stock.Indicators;

public readonly record struct AdlResult
(
    DateTime Timestamp,
    double Adl,
    double? MoneyFlowMultiplier,
    double? MoneyFlowVolume
) : IReusable
{
    double IReusable.Value => Adl;
}
