namespace Skender.Stock.Indicators;

public record struct KamaResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? ER { get; set; }
    public double? Kama { get; set; }

    readonly double IReusableResult.Value
        => Kama.Null2NaN();
}
