namespace Skender.Stock.Indicators;

public record struct VwapResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Vwap { get; set; }

    readonly double IReusableResult.Value
        => Vwap.Null2NaN();
}
