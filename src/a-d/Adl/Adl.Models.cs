namespace Skender.Stock.Indicators;

public interface IAdlResult
{
    public double Adl { get; }
}

[Serializable]
public sealed class AdlResult : ResultBase, IAdlResult, IReusableResult
{
    public double? MoneyFlowMultiplier { get; set; }
    public double? MoneyFlowVolume { get; set; }
    public double Adl { get; set; }
    public double? AdlSma { get; set; }

    double? IReusableResult.Value => Adl;
}
