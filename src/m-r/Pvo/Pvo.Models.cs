namespace Skender.Stock.Indicators;

public record struct PvoResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Pvo { get; set; }
    public double? Signal { get; set; }
    public double? Histogram { get; set; }

    readonly double IReusable.Value
        => Pvo.Null2NaN();
}
