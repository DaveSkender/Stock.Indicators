namespace Skender.Stock.Indicators;

public record struct HurstResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? HurstExponent { get; set; }

    readonly double IReusableResult.Value
        => HurstExponent.Null2NaN();
}
