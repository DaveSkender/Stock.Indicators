namespace Skender.Stock.Indicators;

public record struct RocResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Momentum { get; set; }
    public double? Roc { get; set; }

    readonly double IReusable.Value
        => Roc.Null2NaN();
}
