namespace Skender.Stock.Indicators;

[Serializable]
public sealed class WmaResult : ResultBase, IReusableResult
{
    public double? Wma { get; set; }

    double? IReusableResult.Value => Wma;
}
