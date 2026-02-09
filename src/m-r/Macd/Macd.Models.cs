namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a MACD (Moving Average Convergence Divergence) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the MACD result.</param>
/// <param name="Macd">The MACD value.</param>
/// <param name="Signal">The signal line value.</param>
/// <param name="Histogram">The histogram value.</param>
/// <param name="FastEma">The fast EMA value.</param>
/// <param name="SlowEma">The slow EMA value.</param>
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
    [JsonIgnore]
    public double Value => Macd.Null2NaN();
}
