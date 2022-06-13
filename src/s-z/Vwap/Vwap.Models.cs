namespace Skender.Stock.Indicators;

[Serializable]
public sealed class VwapResult : ResultBase, IReusableResult
{
    public double? Vwap { get; set; }

    double? IReusableResult.Value => Vwap;
}
