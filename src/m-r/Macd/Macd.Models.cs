namespace Skender.Stock.Indicators;

public record MacdResult
(
    DateTime Timestamp,
    double? Macd,
    double? Signal,
    double? Histogram,

    // extra interim data
    double? FastEma,
    double? SlowEma

) : Reusable(Timestamp, Macd.Null2NaN());
