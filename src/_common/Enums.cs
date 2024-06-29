namespace Skender.Stock.Indicators;

// SHARED ENUMERATIONS
// note: indicator unique ENUMS specified in indicator models

/// <summary>
/// Cache action instruction or outcome
/// </summary>
public enum Act
{
    AddNew = 0,  // default
    AddOld,
    Update,
    Delete,
    DoNothing
}

/// <summary>
/// Part or value of a quote candle
/// </summary>
public enum CandlePart
{
    Close = 0,          // default
    Open,
    High,
    Low,
    Volume,
    Hl2,
    Hlc3,
    Oc2,
    Ohl3,
    Ohlc4
}

/// <summary>
/// Candle close or high/low wick values
/// </summary>
public enum EndType
{
    Close = 0,           // default
    HighLow = 1
}

/// <summary>
/// Candlestick pattern matching type
/// </summary>
public enum Match
{
    BullConfirmed = 200,
    BullSignal = 100,
    BullBasis = 10,
    Neutral = 1,
    None = 0,            // default
    BearBasis = -10,
    BearSignal = -100,
    BearConfirmed = -200
}

/// <summary>
/// Moving average type
/// </summary>
public enum MaType
{
    Sma = 0,             // default
    Alma,
    Dema,
    Epma,
    Ema,
    Hma,
    Kama,
    Mama,
    Smma,
    Tema,
    Wma
}

/// <summary>
/// Period size.  Usually referring to the
/// time period represented in a quote candle.
/// </summary>
public enum PeriodSize
{
    Day = 0,           // default
    Week,
    Month,
    FourHours,
    TwoHours,
    OneHour,
    ThirtyMinutes,
    FifteenMinutes,
    FiveMinutes,
    ThreeMinutes,
    TwoMinutes,
    OneMinute
}
