namespace Skender.Stock.Indicators;

[Serializable]
public record AdlResult
(
    DateTime Timestamp,
    double Adl,
    double? MoneyFlowMultiplier = null,
    double? MoneyFlowVolume = null
) : IReusable
{
    public double Value => Adl;
}
