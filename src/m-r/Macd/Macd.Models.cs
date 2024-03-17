namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class MacdResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Macd { get; set; }
    public double? Signal { get; set; }
    public double? Histogram { get; set; }

    // extra interim data
    public double? FastEma { get; set; }
    public double? SlowEma { get; set; }

    double IReusableResult.Value => Macd.Null2NaN();
}
