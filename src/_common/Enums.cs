namespace Skender.Stock.Indicators;

/// <summary>
/// Cache action instruction or outcome.
/// </summary>
internal enum Act
{
    /// <summary>
    /// Adds item to end of cache or rebuild if older.
    /// </summary>
    Add = 0,

    /// <summary>
    /// Does nothing to cache (aborted).
    /// </summary>
    Ignore = 1,

    /// <summary>
    /// Insert item without rebuilding cache.
    /// </summary>
    Insert = 2,

    /// <summary>
    /// Reset and rebuild from marker position.
    /// </summary>
    Rebuild = 3
}

/// <summary>
/// Part or value of a quote candle.
/// </summary>
public enum CandlePart
{
    /// <summary>
    /// Opening price.
    /// </summary>
    Open = 0,

    /// <summary>
    /// Highest price.
    /// </summary>
    High = 1,

    /// <summary>
    /// Lowest price.
    /// </summary>
    Low = 2,

    /// <summary>
    /// Closing price.
    /// </summary>
    Close = 3,

    /// <summary>
    /// Volume of trades.
    /// </summary>
    Volume = 4,

    /// <summary>
    /// Average of high and low prices.
    /// </summary>
    HL2 = 5,

    /// <summary>
    /// Average of high, low, and close prices.
    /// </summary>
    HLC3 = 6,

    /// <summary>
    /// Average of open and close prices.
    /// </summary>
    OC2 = 7,

    /// <summary>
    /// Average of open, high, and low prices.
    /// </summary>
    OHL3 = 8,

    /// <summary>
    /// Average of open, high, low, and close prices.
    /// </summary>
    OHLC4 = 9
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
    /// Strong bearish confirmation.
    /// </summary>
    BearConfirmed = -200,

    /// <summary>
    /// Bearish signal.
    /// </summary>
    BearSignal = -100,

    /// <summary>
    /// Bearish basis.
    /// </summary>
    BearBasis = -10,

    /// <summary>
    /// No pattern.
    /// </summary>
    None = 0,

    /// <summary>
    /// Neutral pattern.
    /// </summary>
    Neutral = 1,

    /// <summary>
    /// Bullish basis.
    /// </summary>
    BullBasis = 10,

    /// <summary>
    /// Bullish signal.
    /// </summary>
    BullSignal = 100,
    /// <summary>
    /// Strong bullish confirmation.
    /// </summary>
    BullConfirmed = 200
}

/// <summary>
/// Moving average type.
/// </summary>
public enum MaType
{
    /// <summary>
    /// Arnaud Legoux Moving Average.
    /// </summary>
    ALMA = 0,

    /// <summary>
    /// Double Exponential Moving Average.
    /// </summary>
    DEMA = 1,

    /// <summary>
    /// Exponential Percentage Moving Average.
    /// </summary>
    EPMA = 2,

    /// <summary>
    /// Exponential Moving Average.
    /// </summary>
    EMA = 3,

    /// <summary>
    /// Hull Moving Average.
    /// </summary>
    HMA = 4,

    /// <summary>
    /// Kaufman Adaptive Moving Average.
    /// </summary>
    KAMA = 5,

    /// <summary>
    /// MESA Adaptive Moving Average.
    /// </summary>
    MAMA = 6,

    /// <summary>
    /// Simple Moving Average.
    /// </summary>
    SMA = 7,

    /// <summary>
    /// Smoothed Moving Average.
    /// </summary>
    SMMA = 8,

    /// <summary>
    /// Triple Exponential Moving Average.
    /// </summary>
    TEMA = 9,

    /// <summary>
    /// Weighted Moving Average.
    /// </summary>
    WMA = 10
}

/// <summary>
/// String output format type.
/// </summary>
public enum OutType
{
    /// <summary>
    /// Fixed width format.
    /// </summary>
    FixedWidth = 0,

    /// <summary>
    /// Comma-separated values format.
    /// </summary>
    CSV = 1,

    /// <summary>
    /// JSON format.
    /// </summary>
    JSON = 2
}

/// <summary>
/// Period size, usually referring to the time period represented in a quote candle.
/// </summary>
public enum PeriodSize
{
    /// <summary>
    /// Monthly period.
    /// </summary>
    Month = 0,

    /// <summary>
    /// Weekly period.
    /// </summary>
    Week = 1,

    /// <summary>
    /// Daily period.
    /// </summary>
    Day = 2,

    /// <summary>
    /// Four-hour period.
    /// </summary>
    FourHours = 3,

    /// <summary>
    /// Two-hour period.
    /// </summary>
    TwoHours = 4,

    /// <summary>
    /// One-hour period.
    /// </summary>
    OneHour = 5,

    /// <summary>
    /// Thirty-minute period.
    /// </summary>
    ThirtyMinutes = 6,

    /// <summary>
    /// Fifteen-minute period.
    /// </summary>
    FifteenMinutes = 7,

    /// <summary>
    /// Five-minute period.
    /// </summary>
    FiveMinutes = 8,

    /// <summary>
    /// Three-minute period.
    /// </summary>
    ThreeMinutes = 9,

    /// <summary>
    /// Two-minute period.
    /// </summary>
    TwoMinutes = 10,

    /// <summary>
    /// One-minute period.
    /// </summary>
    OneMinute = 11
}

/// <summary>
/// Specifies the position direction.
/// </summary>
public enum Direction
{
    /// <summary>
    /// Represents a long position.
    /// </summary>
    Long = 0,

    /// <summary>
    /// Represents a short position.
    /// </summary>
    Short = 1
}
