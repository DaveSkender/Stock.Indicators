namespace Skender.Stock.Indicators;

public record struct PvoResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Pvo { get; set; }
    public double? Signal { get; set; }
    public double? Histogram { get; set; }

    readonly double IReusableResult.Value
        => Pvo.Null2NaN();
}
