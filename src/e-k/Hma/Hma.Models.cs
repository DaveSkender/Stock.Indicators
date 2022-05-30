namespace Skender.Stock.Indicators;

[Serializable]
public sealed class HmaResult : ResultBase, IReusableResult
{
    public double? Hma { get; set; }

    double? IReusableResult.Value => Hma;
}
