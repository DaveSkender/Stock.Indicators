namespace Skender.Stock.Indicators;

[Serializable]
public sealed class EpmaResult : ResultBase, IReusableResult
{
    public double? Epma { get; set; }

    double? IReusableResult.Value => Epma;
}
