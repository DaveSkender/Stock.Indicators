namespace Skender.Stock.Indicators;

public readonly record struct SuperTrendResult
(
    DateTime Timestamp,
    decimal? SuperTrend,
    decimal? UpperBand,
    decimal? LowerBand
) : IResult;
