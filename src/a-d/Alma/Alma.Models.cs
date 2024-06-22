namespace Skender.Stock.Indicators;

public record struct AlmaResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Alma { get; set; }

    readonly double IReusableResult.Value
        => Alma.Null2NaN();
}
