/// <summary>
/// Cache action instruction or outcome.
/// </summary>
internal enum Act
{
    /// <summary>
    /// Adds item to end of cache or rebuild if older.
    /// </summary>
    Add,

    /// <summary>
    /// Does nothing to cache (aborted).
    /// </summary>
    Ignore,

    /// <summary>
    /// Insert item without rebuilding cache.
    /// </summary>
    Insert,

    /// <summary>
    /// Reset and rebuild from marker position.
    /// </summary>
    Rebuild
}

/// <summary>
/// Part or value of a quote candle.
/// </summary>
public enum CandlePart
{
    /// <summary>
    /// Opening price.
    /// </summary>
    Open,

    /// <summary>
    /// Highest price.
    /// </summary>
    High,

    /// <summary>
    /// Lowest price.
    /// </summary>
    Low,

    /// <summary>
    /// Closing price.
    /// </summary>
    Close,

    /// <summary>
    /// Volume of trades.
    /// </summary>
    Volume,

    /// <summary>
    /// Average of high and low prices.
    /// </summary>
    HL2,

    /// <summary>
    /// Average of high, low, and close prices.
    /// </summary>
    HLC3,

    /// <summary>
    /// Average of open and close prices.
    /// </summary>
    OC2,

    /// <summary>
    /// Average of open, high, and low prices.
    /// </summary>
    OHL3,

    /// <summary>
    /// Average of open, high, low, and close prices.
    /// </summary>
    OHLC4
}

/// <summary>
/// Price candle end types used to select which aspect of the candle
/// to use in indicator threshold calculations.
/// </summary>
public enum EndType
{
    /// <summary>
    /// Closing price.
    /// </summary>
    Close = 0,

    /// <summary>
    /// High and low prices.
    /// </summary>
    HighLow = 1
}

/// <summary>
/// Candlestick pattern matching type.
/// </summary>
public enum Match
{
    /// <summary>
    /// Strong bullish confirmation.
    /// </summary>
    BullConfirmed = 200,

    /// <summary>
    /// Bullish signal.
    /// </summary>
    BullSignal = 100,

    /// <summary>
    /// Bullish basis.
    /// </summary>
    BullBasis = 10,

    /// <summary>
    /// Neutral pattern.
    /// </summary>
    Neutral = 1,

    /// <summary>
    /// No pattern.
    /// </summary>
    None = 0,

    /// <summary>
    /// Bearish basis.
    /// </summary>
    BearBasis = -10,

    /// <summary>
    /// Bearish signal.
    /// </summary>
    BearSignal = -100,

    /// <summary>
    /// Strong bearish confirmation.
    /// </summary>
    BearConfirmed = -200
}

/// <summary>
/// Moving average type.
/// </summary>
public enum MaType
{
    /// <summary>
    /// Arnaud Legoux Moving Average.
    /// </summary>
    ALMA,

    /// <summary>
    /// Double Exponential Moving Average.
    /// </summary>
    DEMA,

    /// <summary>
    /// Exponential Percentage Moving Average.
    /// </summary>
    EPMA,

    /// <summary>
    /// Exponential Moving Average.
    /// </summary>
    EMA,

    /// <summary>
    /// Hull Moving Average.
    /// </summary>
    HMA,

    /// <summary>
    /// Kaufman Adaptive Moving Average.
    /// </summary>
    KAMA,

    /// <summary>
    /// MESA Adaptive Moving Average.
    /// </summary>
    MAMA,

    /// <summary>
    /// Simple Moving Average.
    /// </summary>
    SMA,

    /// <summary>
    /// Smoothed Moving Average.
    /// </summary>
    SMMA,

    /// <summary>
    /// Triple Exponential Moving Average.
    /// </summary>
    TEMA,

    /// <summary>
    /// Weighted Moving Average.
    /// </summary>
    WMA
}

/// <summary>
/// String output format type.
/// </summary>
public enum OutType
{
    /// <summary>
    /// Fixed width format.
    /// </summary>
    FixedWidth,

    /// <summary>
    /// Comma-separated values format.
    /// </summary>
    CSV,

    /// <summary>
    /// JSON format.
    /// </summary>
    JSON
}

/// <summary>
/// Period size, usually referring to the time period represented in a quote candle.
/// </summary>
public enum PeriodSize
{
    /// <summary>
    /// Monthly period.
    /// </summary>
    Month,

    /// <summary>
    /// Weekly period.
    /// </summary>
    Week,

    /// <summary>
    /// Daily period.
    /// </summary>
    Day,

    /// <summary>
    /// Four-hour period.
    /// </summary>
    FourHours,

    /// <summary>
    /// Two-hour period.
    /// </summary>
    TwoHours,

    /// <summary>
    /// One-hour period.
    /// </summary>
    OneHour,

    /// <summary>
    /// Thirty-minute period.
    /// </summary>
    ThirtyMinutes,

    /// <summary>
    /// Fifteen-minute period.
    /// </summary>
    FifteenMinutes,

    /// <summary>
    /// Five-minute period.
    /// </summary>
    FiveMinutes,

    /// <summary>
    /// Three-minute period.
    /// </summary>
    ThreeMinutes,

    /// <summary>
    /// Two-minute period.
    /// </summary>
    TwoMinutes,

    /// <summary>
    /// One-minute period.
    /// </summary>
    OneMinute
}
