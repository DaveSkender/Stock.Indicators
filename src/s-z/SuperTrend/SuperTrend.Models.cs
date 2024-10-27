namespace Skender.Stock.Indicators;

[Serializable]
public record SuperTrendResult
(
    DateTime Timestamp,
    decimal? SuperTrend,
    decimal? UpperBand,
    decimal? LowerBand
) : ISeries;
