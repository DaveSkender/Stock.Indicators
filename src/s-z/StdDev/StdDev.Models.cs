namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class StdDevResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? StdDev { get; set; }
    public double? Mean { get; set; }
    public double? ZScore { get; set; }

    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average signal.", false)]
    public double? StdDevSma { get; set; }

    double IReusableResult.Value => StdDev.Null2NaN();
}
