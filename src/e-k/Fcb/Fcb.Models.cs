namespace Skender.Stock.Indicators;

public readonly record struct FcbResult
(
    DateTime Timestamp,
    decimal? UpperBand,
    decimal? LowerBand
) : IResult;
