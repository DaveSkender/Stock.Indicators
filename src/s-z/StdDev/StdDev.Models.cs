namespace Skender.Stock.Indicators;

[Serializable]
public sealed class StdDevResult : ResultBase, IReusableResult
{
    public StdDevResult(DateTime date)
    {
        Date = date;
    }

    public double? StdDev { get; set; }
    public double? Mean { get; set; }
    public double? ZScore { get; set; }

    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average signal.", false)]
    public double? StdDevSma { get; set; }

    double IReusableResult.Value => StdDev.Null2NaN();
}
