namespace Skender.Stock.Indicators;

// SHARED ENUMERATIONS
// note: indicator unique ENUMS specified in indicator models

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

public enum EndType
{
    Close = 0,
    HighLow = 1
}

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

public enum SyncType
{
    Prepend,
    AppendOnly,
    RemoveOnly,
    FullMatch
}