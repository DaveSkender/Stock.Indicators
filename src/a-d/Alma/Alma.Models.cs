namespace Skender.Stock.Indicators;

[Serializable]
public sealed class AlmaResult : ResultBase, IReusableResult
{
    public double? Alma { get; set; }

    double? IReusableResult.Value => Alma;
}
