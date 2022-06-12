namespace Skender.Stock.Indicators;

[Serializable]
public sealed class RocResult : ResultBase, IReusableResult
{
    public double? Roc { get; set; }
    public double? RocSma { get; set; }

    double? IReusableResult.Value => Roc;
}

[Serializable]
public sealed class RocWbResult : ResultBase, IReusableResult
{
    public double? Roc { get; set; }
    public double? RocEma { get; set; }
    public double? UpperBand { get; set; }
    public double? LowerBand { get; set; }

    double? IReusableResult.Value => Roc;
}
