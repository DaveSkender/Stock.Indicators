namespace Skender.Stock.Indicators;

public record struct DynamicResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Dynamic { get; set; }

    readonly double IReusable.Value
        => Dynamic.Null2NaN();
}
