namespace Skender.Stock.Indicators;

[Serializable]
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
    /// <inheritdoc/>
    public double Value => Macd.Null2NaN();
}
