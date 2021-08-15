namespace Skender.Stock.Indicators
{
    // ENUMERATIONS

    internal enum DivergenceType
    {
        // TODO: not implemented
        RegularBull,
        RegularBear,
        HiddenBull,
        HiddenBear
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

    internal enum QuoteType
    {
        // TODO: not implemented
        AllowNulls,
        ExcludeNull
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
