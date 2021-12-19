namespace Skender.Stock.Indicators
{
    // SHARED ENUMERATIONS
    // note: indicator unique ENUMS specified in indicator models

    public enum CandlePart
    {
        Open,
        High,
        Low,
        Close,
        Volume,
        HL2
    }

    public enum EndType
    {
        Close = 0,
        HighLow = 1
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
}
