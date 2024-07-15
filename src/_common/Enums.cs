namespace Skender.Stock.Indicators;

// SHARED ENUMERATIONS
// note: indicator unique ENUMS specified in indicator models

/// <summary>
/// Cache action instruction or outcome
/// </summary>
public enum Act
{
    /// <summary>
    /// Adds to end of cache
    /// </summary>
    AddNew,

    /// <summary>
    /// Adds new item to middle of cache
    /// </summary>
    AddOld,

    /// <summary>
    /// Updates existing item in cache
    /// </summary>
    Update,

    /// <summary>
    /// Deletes existing item in cache
    /// </summary>
    Delete,

    /// <summary>
    /// Does nothing to cache (aborted)
    /// </summary>
    DoNothing,

    /// <summary>
    /// Delete from first position of cache
    /// without rebuilding or recalculating;
    /// as part of the auto-pruning process
    /// to maintain maximum cache size.
    /// </summary>
    AutoPrune,  // TODO: implement. May also have some integrity checks.

    /// <summary>
    /// Instruction has not yet been determined
    /// </summary>
    Unknown
}

/// <summary>
/// Part or value of a quote candle
/// </summary>
public enum CandlePart
{
    Open,
    High,
    Low,
    Close,
    Volume,
    HL2,
    HLC3,
    OC2,
    OHL3,
    OHLC4
}

/// <summary>
/// Candle close or high/low wick values
/// </summary>
public enum EndType
{
    Close = 0,
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
    None = 0,
    BearBasis = -10,
    BearSignal = -100,
    BearConfirmed = -200
}

/// <summary>
/// Moving average type
/// </summary>
public enum MaType
{
    ALMA,
    DEMA,
    EPMA,
    EMA,
    HMA,
    KAMA,
    MAMA,
    SMA,
    SMMA,
    TEMA,
    WMA
}

/// <summary>
/// Period size.  Usually referring to the
/// time period represented in a quote candle.
/// </summary>
public enum PeriodSize
{
    Month,
    Week,
    Day,
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
