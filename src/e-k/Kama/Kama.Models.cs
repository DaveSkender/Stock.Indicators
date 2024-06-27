namespace Skender.Stock.Indicators;

public record struct KamaResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? ER { get; set; }
    public double? Kama { get; set; }

    readonly double IReusable.Value
        => Kama.Null2NaN();
}
