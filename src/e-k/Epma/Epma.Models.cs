namespace Skender.Stock.Indicators;

public record struct EpmaResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Epma { get; set; }

    readonly double IReusable.Value
        => Epma.Null2NaN();
}
