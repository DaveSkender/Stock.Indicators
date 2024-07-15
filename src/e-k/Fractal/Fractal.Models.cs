namespace Skender.Stock.Indicators;

public record FractalResult
(
    DateTime Timestamp,
    decimal? FractalBear,
    decimal? FractalBull
) : ISeries;
