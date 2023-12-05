namespace Skender.Stock.Indicators;

public sealed class RocWbResult : ResultBase, IReusableResult
{
    public double? Roc { get; set; }
    public double? RocEma { get; set; }
    public double? UpperBand { get; set; }
    public double? LowerBand { get; set; }

    double IReusableResult.Value => Roc.Null2NaN();
}
