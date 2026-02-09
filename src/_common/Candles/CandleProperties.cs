namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the properties of a candlestick.
/// </summary>
/// <param name="Timestamp">The timestamp of the candlestick.</param>
/// <param name="Open">The opening price of the candlestick.</param>
/// <param name="High">The highest price of the candlestick.</param>
/// <param name="Low">The lowest price of the candlestick.</param>
/// <param name="Close">The closing price of the candlestick.</param>
/// <param name="Volume">The volume of the candlestick.</param>
[Serializable]
public record CandleProperties
(
    DateTime Timestamp,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume
) : Quote(Timestamp, Open, High, Low, Close, Volume)
{
    /// <summary>
    /// Gets the size of the candlestick.
    /// </summary>
    public decimal? Size => High - Low;

    /// <summary>
    /// Gets the body size of the candlestick.
    /// </summary>
    public decimal? Body => Open > Close ? Open - Close : Close - Open;

    /// <summary>
    /// Gets the size of the upper wick of the candlestick.
    /// </summary>
    public decimal? UpperWick => High - (Open > Close ? Open : Close);

    /// <summary>
    /// Gets the size of the lower wick of the candlestick.
    /// </summary>
    public decimal? LowerWick => (Open > Close ? Close : Open) - Low;

    /// <summary>
    /// Gets the body size as a percentage of the total size.
    /// </summary>
    public double? BodyPct => Size != 0 ? (double?)(Body / Size) : 1;

    /// <summary>
    /// Gets the upper wick size as a percentage of the total size.
    /// </summary>
    public double? UpperWickPct => Size != 0 ? (double?)(UpperWick / Size) : 1;

    /// <summary>
    /// Gets the lower wick size as a percentage of the total size.
    /// </summary>
    public double? LowerWickPct => Size != 0 ? (double?)(LowerWick / Size) : 1;

    /// <summary>
    /// Gets a value indicating whether the candlestick is bullish.
    /// </summary>
    public bool IsBullish => Close > Open;

    /// <summary>
    /// Gets a value indicating whether the candlestick is bearish.
    /// </summary>
    public bool IsBearish => Close < Open;
}
