namespace Skender.Stock.Indicators;

public record struct VwmaResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Vwma { get; set; }

    readonly double IReusableResult.Value
        => Vwma.Null2NaN();
}
