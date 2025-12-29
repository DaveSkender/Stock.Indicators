namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Fractal Channel Band (FCB) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="UpperBand">The upper band value.</param>
/// <param name="LowerBand">The lower band value.</param>
[Serializable]
public record FcbResult
(
    DateTime Timestamp,
    decimal? UpperBand,
    decimal? LowerBand
) : ISeries;
