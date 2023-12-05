namespace Skender.Stock.Indicators;

public sealed class MacdResult : ResultBase, IReusableResult
{
    public double? Macd { get; set; }
    public double? Signal { get; set; }
    public double? Histogram { get; set; }

    // extra interim data
    public double? FastEma { get; set; }
    public double? SlowEma { get; set; }

    double IReusableResult.Value => Macd.Null2NaN();
}
