namespace Skender.Stock.Indicators;

public record struct PmoResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Pmo { get; set; }
    public double? Signal { get; set; }

    readonly double IReusable.Value
        => Pmo.Null2NaN();
}
