namespace Skender.Stock.Indicators;

public record struct DynamicResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Dynamic { get; set; }

    readonly double IReusableResult.Value
        => Dynamic.Null2NaN();
}
