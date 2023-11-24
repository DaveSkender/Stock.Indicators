namespace Skender.Stock.Indicators;

[Serializable]
public sealed class RocResult : ResultBase, IReusableResult
{
    public RocResult(DateTime date)
    {
        Date = date;
    }

    public double? Momentum { get; set; }
    public double? Roc { get; set; }

    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average signal.", false)]
    public double? RocSma { get; set; }

    double IReusableResult.Value => Roc.Null2NaN();
}
