namespace Skender.Stock.Indicators;

[Serializable]
public sealed class DemaResult : ResultBase, IReusableResult
{
    public double? Dema { get; set; }

    double? IReusableResult.Value => Dema;
}
