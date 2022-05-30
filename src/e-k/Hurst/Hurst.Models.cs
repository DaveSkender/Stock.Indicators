namespace Skender.Stock.Indicators;

[Serializable]
public sealed class HurstResult : ResultBase, IReusableResult
{
    public double? HurstExponent { get; set; }

    double? IReusableResult.Value => HurstExponent;
}
