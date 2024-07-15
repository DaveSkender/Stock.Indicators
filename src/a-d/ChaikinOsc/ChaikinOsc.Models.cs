namespace Skender.Stock.Indicators;

public record ChaikinOscResult
(
    DateTime Timestamp,
    double? MoneyFlowMultiplier,
    double? MoneyFlowVolume,
    double? Adl,
    double? Oscillator
) : Reusable(Timestamp)
{
    public override double Value => Oscillator.Null2NaN();
}
