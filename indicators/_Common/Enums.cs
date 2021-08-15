namespace Skender.Stock.Indicators
{
    // ENUMERATIONS

    public enum DivergenceType
    {
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
        HH,
        LH,
        HL,
        LL
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
