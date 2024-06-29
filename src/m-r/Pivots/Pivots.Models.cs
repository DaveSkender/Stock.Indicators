namespace Skender.Stock.Indicators;

public record struct PivotsResult
(
    DateTime Timestamp,
    decimal? HighPoint,
    decimal? LowPoint,
    decimal? HighLine,
    decimal? LowLine,
    PivotTrend? HighTrend,
    PivotTrend? LowTrend
) : IResult;

public enum PivotTrend
{
    HH, // higher high
    LH, // lower high
    HL, // higher low
    LL  // lower low
}
