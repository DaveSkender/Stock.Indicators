namespace Skender.Stock.Indicators;

[Serializable]
public class PivotsResult : ResultBase
{
    public PivotsResult(DateTime date)
    {
        Date = date;
    }

    public decimal? HighPoint { get; set; }
    public decimal? LowPoint { get; set; }
    public decimal? HighLine { get; set; }
    public decimal? LowLine { get; set; }
    public PivotTrend? HighTrend { get; set; }
    public PivotTrend? LowTrend { get; set; }
}

public enum PivotTrend
{
    HH, // higher high
    LH, // lower high
    HL, // higher low
    LL // lower low
}
