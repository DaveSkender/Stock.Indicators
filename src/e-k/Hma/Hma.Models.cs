namespace Skender.Stock.Indicators;

public record struct HmaResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Hma { get; set; }

    readonly double IReusable.Value
        => Hma.Null2NaN();
}
