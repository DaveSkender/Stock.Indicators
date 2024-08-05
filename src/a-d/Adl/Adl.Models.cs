namespace Skender.Stock.Indicators;

public record AdlResult
(
    DateTime Timestamp,
    double Adl,
    double? MoneyFlowMultiplier = null,
    double? MoneyFlowVolume = null
) : Reusable(Timestamp)
{
    public override double Value => Adl;
}
