namespace Skender.Stock.Indicators;

public record struct MacdResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Macd { get; set; }
    public double? Signal { get; set; }
    public double? Histogram { get; set; }

    // extra interim data
    public double? FastEma { get; set; }
    public double? SlowEma { get; set; }

    readonly double IReusable.Value
        => Macd.Null2NaN();
}
