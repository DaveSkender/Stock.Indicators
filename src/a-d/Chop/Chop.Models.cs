namespace Skender.Stock.Indicators;

public record struct ChopResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Chop { get; set; }

    readonly double IReusableResult.Value
        => Chop.Null2NaN();
}
