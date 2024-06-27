namespace Skender.Stock.Indicators;

public record struct VwmaResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Vwma { get; set; }

    readonly double IReusable.Value
        => Vwma.Null2NaN();
}
