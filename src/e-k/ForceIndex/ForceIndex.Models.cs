namespace Skender.Stock.Indicators;

public record struct ForceIndexResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? ForceIndex { get; set; }

    readonly double IReusableResult.Value
        => ForceIndex.Null2NaN();
}
