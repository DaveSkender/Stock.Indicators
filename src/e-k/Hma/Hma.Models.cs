namespace Skender.Stock.Indicators;

public record struct HmaResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Hma { get; set; }

    readonly double IReusableResult.Value
        => Hma.Null2NaN();
}
