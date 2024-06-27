namespace Skender.Stock.Indicators;

public record struct WmaResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Wma { get; set; }

    readonly double IReusable.Value
        => Wma.Null2NaN();
}
