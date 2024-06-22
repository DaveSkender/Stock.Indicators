namespace Skender.Stock.Indicators;

public record struct RocResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Momentum { get; set; }
    public double? Roc { get; set; }

    readonly double IReusableResult.Value
        => Roc.Null2NaN();
}
