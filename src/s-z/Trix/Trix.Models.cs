namespace Skender.Stock.Indicators;

public record struct TrixResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Ema3 { get; set; }
    public double? Trix { get; set; }

    readonly double IReusableResult.Value
        => Trix.Null2NaN();
}
