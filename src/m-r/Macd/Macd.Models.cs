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

/// <summary>
/// Represents the result of a MACD (Moving Average Convergence Divergence) calculation, including the MACD value,
/// signal line, histogram, and related exponential moving averages for a specific point in time.
/// </summary>
/// <param name="Timestamp">The date and time associated with this MACD result.</param>
/// <param name="Macd">The calculated MACD value at the specified timestamp.</param>
/// <param name="Signal">The value of the signal line, typically an EMA of the MACD, at the specified timestamp.</param>
/// <param name="Histogram">The difference between the MACD and the signal line at the specified timestamp.</param>
/// <param name="FastEma">The value of the fast (short-period) exponential moving average used in the MACD calculation.</param>
/// <param name="SlowEma">The value of the slow (long-period) exponential moving average used in the MACD calculation.</param>
public record MacdResultX
(
    DateTime Timestamp,
    long Macd,
    long Signal,
    long Histogram,

    // extra/interim data
    long FastEma,
    long SlowEma

) : IReusableX
{
    /// <inheritdoc/>
    [JsonIgnore]
    public long Value => Macd;
}
