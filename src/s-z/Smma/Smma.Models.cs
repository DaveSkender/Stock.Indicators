namespace Skender.Stock.Indicators;

[Serializable]
public sealed class SmmaResult : ResultBase, IReusableResult
{
    public double? Smma { get; set; }

    double? IReusableResult.Value => Smma;
}
