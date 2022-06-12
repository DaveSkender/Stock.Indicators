namespace Skender.Stock.Indicators;

[Serializable]
public sealed class PmoResult : ResultBase, IReusableResult
{
    public double? Pmo { get; set; }
    public double? Signal { get; set; }

    // internal use only
    internal double? RocEma { get; set; }

    double? IReusableResult.Value => Pmo;
}
