namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Fractal calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="FractalBear">The Fractal Bear value.</param>
/// <param name="FractalBull">The Fractal Bull value.</param>
[Serializable]
public record FractalResult
(
    DateTime Timestamp,
    decimal? FractalBear,
    decimal? FractalBull
) : ISeries;
