namespace Skender.Stock.Indicators;

public record struct TrixResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Ema3 { get; set; }
    public double? Trix { get; set; }

    readonly double IReusable.Value
        => Trix.Null2NaN();
}
