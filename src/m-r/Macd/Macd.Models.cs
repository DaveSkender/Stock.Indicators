namespace Skender.Stock.Indicators;

public readonly record struct MacdResult
(
    DateTime Timestamp,
    double? Macd,
    double? Signal,
    double? Histogram,

    // extra interim data
    double? FastEma,
    double? SlowEma

) : IReusable
{
    double IReusable.Value => Macd.Null2NaN();
}
