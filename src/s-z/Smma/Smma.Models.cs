namespace Skender.Stock.Indicators;

public record struct SmmaResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Smma { get; set; }

    readonly double IReusable.Value
        => Smma.Null2NaN();
}
