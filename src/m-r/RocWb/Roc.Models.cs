namespace Skender.Stock.Indicators;

[Serializable]
public class RocWbResult : ResultBase, IReusableResult
{
    public RocWbResult(DateTime date)
    {
        Date = date;
    }

    public double? Roc { get; set; }
    public double? RocEma { get; set; }
    public double? UpperBand { get; set; }
    public double? LowerBand { get; set; }

    double? IReusableResult.Value => Roc;
}
