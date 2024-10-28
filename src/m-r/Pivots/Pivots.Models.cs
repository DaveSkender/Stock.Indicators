namespace Skender.Stock.Indicators;

[Serializable]
public record struct PivotsResult
(
    DateTime Timestamp,
    decimal? HighPoint,
    decimal? LowPoint,
    decimal? HighLine,
    decimal? LowLine,
    PivotTrend? HighTrend,
    PivotTrend? LowTrend
) : ISeries;

public enum PivotTrend
{
    Hh, // higher high
    Lh, // lower high
    Hl, // higher low
    Ll  // lower low
}
