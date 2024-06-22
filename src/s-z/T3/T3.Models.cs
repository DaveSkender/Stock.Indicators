namespace Skender.Stock.Indicators;

public record struct T3Result : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? T3 { get; set; }

    readonly double IReusableResult.Value
        => T3.Null2NaN();
}
