namespace Skender.Stock.Indicators;

public record struct WmaResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Wma { get; set; }

    readonly double IReusableResult.Value
        => Wma.Null2NaN();
}
