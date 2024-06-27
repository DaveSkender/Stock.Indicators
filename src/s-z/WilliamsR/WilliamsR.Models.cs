namespace Skender.Stock.Indicators;

public record struct WilliamsResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? WilliamsR { get; set; }

    readonly double IReusable.Value
        => WilliamsR.Null2NaN();
}
