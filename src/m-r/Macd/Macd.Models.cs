namespace Skender.Stock.Indicators;

public record MacdResult
(
    DateTime Timestamp,
    double? Macd,
    double? Signal,
    double? Histogram,

    // extra/interim data
    double? FastEma,
    double? SlowEma

) : IReusable
{
    public double Value => Macd.Null2NaN();
}
