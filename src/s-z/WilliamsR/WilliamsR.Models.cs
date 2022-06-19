namespace Skender.Stock.Indicators;

[Serializable]
public sealed class WilliamsResult : ResultBase, IReusableResult
{
    public double? WilliamsR { get; set; }

    double? IReusableResult.Value => WilliamsR;
}
