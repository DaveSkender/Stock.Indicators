namespace Skender.Stock.Indicators;

public record struct SmmaResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Smma { get; set; }

    readonly double IReusableResult.Value
        => Smma.Null2NaN();
}
