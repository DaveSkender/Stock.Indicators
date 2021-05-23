namespace Skender.Stock.Indicators
{
    // ENUMERATIONS

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
        Hour
    }

    public enum EndType
    {
        Close = 0,
        HighLow = 1
    }

    public enum DivergenceType
    {
        RegularBull,
        RegularBear,
        HiddenBull,
        HiddenBear
    }
}
