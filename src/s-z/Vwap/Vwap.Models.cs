namespace Skender.Stock.Indicators;

public record struct VwapResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Vwap { get; set; }

    readonly double IReusable.Value
        => Vwap.Null2NaN();
}
