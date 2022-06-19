namespace Skender.Stock.Indicators;

[Serializable]
public sealed class ForceIndexResult : ResultBase, IReusableResult
{
    public double? ForceIndex { get; set; }

    double? IReusableResult.Value => ForceIndex;
}
