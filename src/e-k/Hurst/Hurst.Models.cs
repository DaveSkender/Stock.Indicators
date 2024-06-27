namespace Skender.Stock.Indicators;

public record struct HurstResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? HurstExponent { get; set; }

    readonly double IReusable.Value
        => HurstExponent.Null2NaN();
}
