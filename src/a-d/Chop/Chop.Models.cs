namespace Skender.Stock.Indicators;

public record struct ChopResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Chop { get; set; }

    readonly double IReusable.Value
        => Chop.Null2NaN();
}
