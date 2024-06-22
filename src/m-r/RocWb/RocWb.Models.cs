namespace Skender.Stock.Indicators;

public record struct RocWbResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Roc { get; set; }
    public double? RocEma { get; set; }
    public double? UpperBand { get; set; }
    public double? LowerBand { get; set; }

    readonly double IReusableResult.Value
        => Roc.Null2NaN();
}
