namespace Skender.Stock.Indicators;

[Serializable]
public sealed class AdlResult : ResultBase, IReusableResult
{
    public double? MoneyFlowMultiplier { get; set; }
    public double? MoneyFlowVolume { get; set; }
    public double Adl { get; set; }
    public double? AdlSma { get; set; }

    double? IReusableResult.Value => Adl;
}
