namespace Skender.Stock.Indicators;

public readonly record struct ChaikinOscResult
(
    DateTime Timestamp,
    double? MoneyFlowMultiplier,
    double? MoneyFlowVolume,
    double? Adl,
    double? Oscillator
) : IReusable
{
    double IReusable.Value => Oscillator.Null2NaN();
}
