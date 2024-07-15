namespace Skender.Stock.Indicators;

public record SuperTrendResult
(
    DateTime Timestamp,
    decimal? SuperTrend,
    decimal? UpperBand,
    decimal? LowerBand
) : ISeries;
