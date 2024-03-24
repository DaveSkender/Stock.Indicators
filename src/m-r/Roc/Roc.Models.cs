namespace Skender.Stock.Indicators;

public sealed record class RocResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Momentum { get; set; }
    public double? Roc { get; set; }

    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average signal.", false)]
    public double? RocSma { get; set; }

    double IReusableResult.Value => Roc.Null2NaN();
}
