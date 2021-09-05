namespace Skender.Stock.Indicators
{
    // ENUMERATIONS

    public enum CandlePart
    {
        Open,
        High,
        Low,
        Close,
        Volume
    }

    public enum EndType
    {
        Close = 0,
        HighLow = 1
    }

    public enum PivotTrend
    {
        HH, // higher high
        LH, // lower high
        HL, // higher low
        LL  // lower low
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
