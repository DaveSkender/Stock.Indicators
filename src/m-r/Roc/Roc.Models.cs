namespace Skender.Stock.Indicators;

[Serializable]
public sealed class RocResult : ResultBase, IReusableResult
{
    public double? Roc { get; set; }
    public double? RocSma { get; set; }

    double? IReusableResult.Value => Roc;
}
