namespace Skender.Stock.Indicators;

public record struct ForceIndexResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? ForceIndex { get; set; }

    readonly double IReusable.Value
        => ForceIndex.Null2NaN();
}
