namespace Skender.Stock.Indicators;

public readonly record struct FractalResult
(
    DateTime Timestamp,
    decimal? FractalBear,
    decimal? FractalBull
) : IResult;
