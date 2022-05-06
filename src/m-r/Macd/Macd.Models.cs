namespace Skender.Stock.Indicators;

[Serializable]
public class MacdResult : ResultBase
{
    public double? Macd { get; set; }
    public double? Signal { get; set; }
    public double? Histogram { get; set; }

    // extra interim data
    public decimal? FastEma { get; set; }
    public decimal? SlowEma { get; set; }
}
