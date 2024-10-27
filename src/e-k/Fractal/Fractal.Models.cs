namespace Skender.Stock.Indicators;

[Serializable]
public record FractalResult
(
    DateTime Timestamp,
    decimal? FractalBear,
    decimal? FractalBull
) : ISeries;
