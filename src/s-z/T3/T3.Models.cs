namespace Skender.Stock.Indicators;

public record struct T3Result : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? T3 { get; set; }

    readonly double IReusable.Value
        => T3.Null2NaN();
}
